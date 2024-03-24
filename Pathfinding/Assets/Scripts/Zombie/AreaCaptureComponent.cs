using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AreaCaptureComponent : BaseCaptureComponent
{
    [SerializeField] float _captureAngle = 90;

    [SerializeField] BaseDrawer _circularSectorDrawer;
    [SerializeField] Transform _sightPoint;

    [SerializeField] List<Transform> _capturedTransforms = new List<Transform>();

    [SerializeField] Transform _storedTarget;

    public override void Initialize(float radius, float angle)
    {
        _captureRadius = radius;
        _captureAngle = angle;
    }

    protected override void OnTargetEnter(Collider other)
    {
        _capturedTransforms.Add(other.transform);
    }

    protected override void OnTargetExit(Collider other)
    {
        _capturedTransforms.Remove(other.transform);
    }

    protected override bool IsAlreadyContaining(Collider other)
    {
        return _capturedTransforms.Contains(other.transform);
    }

    bool CanRaycastTarget(Transform target)
    {
        Vector3 dir = (target.position - _sightPoint.position).normalized;

        RaycastHit hit;
        Physics.Raycast(_sightPoint.position, dir, out hit, _captureRadius);
        Debug.DrawRay(_sightPoint.position, dir * _captureRadius, Color.yellow);

        if (hit.transform == target) return true;
        else return false;
    }

    bool IsInAngle(float angle) { return angle <= _captureAngle / 2 && -_captureAngle / 2 <= angle; }

    // ReturnTargetInSight 사용시 IsTargetInSight 우선 사용
    public Transform ReturnTargetInSight()
    {
        return _storedTarget;
    }

    public bool IsTargetInSight()
    {
        if (_capturedTransforms.Count == 0) return false;

        for (int i = 0; i < _capturedTransforms.Count; i++)
        {
            if (_capturedTransforms[i] == null) continue;

            float angle = ReturnAngleBetween(_capturedTransforms[i].transform.position);
            bool inInAngle = IsInAngle(angle);
            if (inInAngle == false) continue;

            bool canRaycast = CanRaycastTarget(_capturedTransforms[i].transform);
            if (canRaycast == false) continue;

            _storedTarget = _capturedTransforms[i].transform;
            return true;
        }

        return false;
    }

    float ReturnAngleBetween(Vector3 targetPos)
    {
        Vector3 dir = (new Vector3(targetPos.x, transform.position.y, targetPos.z) - transform.position).normalized;
        float angle = Vector3.Angle(transform.forward, dir);
        return angle;
    }

    protected override void OnModifyData()
    {
        base.OnModifyData();
        _circularSectorDrawer.ResetData(_captureAngle, _captureRadius);
    }
}
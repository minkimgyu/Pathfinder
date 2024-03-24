using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

abstract public class BaseCaptureComponent : MonoBehaviour
{

    [SerializeField] protected float _captureRadius;

    [SerializeField] SphereCollider _captureCollider;
    [SerializeField] BaseDrawer _sphereDrawer;
    [SerializeField] LayerMask _layerMask;

    public virtual void Initialize(float radius, int maxQueueSize, Action OnNoiseReceived) { }
    public virtual void Initialize(float radius, float angle) { }

    public void ModifyCaptureRadius(float radius)
    {
        _captureRadius += radius;
        OnModifyData();
    }

    protected virtual void OnTargetEnter(Collider other) { }

    protected virtual void OnTargetExit(Collider other) { }

    protected abstract bool IsAlreadyContaining(Collider other);

    protected bool IsRightLayer(Collider other)
    {
        int value = (_layerMask & (1 << other.gameObject.layer));
        return value != 0;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (IsRightLayer(other) == false || IsAlreadyContaining(other) == true) return;
        OnTargetEnter(other);
    }

    private void OnTriggerExit(Collider other)
    {
        if (IsRightLayer(other) == false || IsAlreadyContaining(other) == false) return;
        OnTargetExit(other);
    }

    protected virtual void OnModifyData()
    {
        if (_sphereDrawer == null) return;
        _sphereDrawer.ResetData(_captureRadius);

        if (_captureCollider == null) return;
        _captureCollider.radius = _captureRadius;
    }

    protected virtual void OnValidate()
    {
        OnModifyData();
    }
}

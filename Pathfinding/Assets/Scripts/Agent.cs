using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Grid.Pathfinder;
using System.Diagnostics;

public class Agent : MonoBehaviour
{
    Func<Vector3, Vector3, List<Vector3>> FindPath;

    List<Vector3> _paths = new List<Vector3>();
    int _pathIndex = 0;
    Vector3 _positionToMove;
    Animator _animator;

    MoveComponent _moveComponent;
    ViewComponent _viewComponent;

    [SerializeField] float _moveSpeed = 5; 
    [SerializeField] float _rotationSpeed = 3;
    [SerializeField] Transform _target;

    // Start is called before the first frame update
    void Start()
    {
        _animator = GetComponent<Animator>();

        _moveComponent = GetComponent<MoveComponent>();
        //_moveComponent.Initialize(_moveSpeed);

        _viewComponent = GetComponent<ViewComponent>();
        _viewComponent.Initialize(_rotationSpeed);

        Pathfinder pathfinder = FindObjectOfType<Pathfinder>();
        if (pathfinder == null) return;

        FindPath = pathfinder.FindPath;
    }

    void ResetPath()
    {
        if (_paths == null || _paths.Count == 0) return;

        _paths.Clear();
        _pathIndex = 0;
    }

    Vector3 ReturnDirection() { return (_positionToMove - transform.position).normalized; }

    bool IsEndOfPath() 
    {
        return _paths == null || _paths.Count == 0 || _paths.Count - 1 < _pathIndex; 
    }

    void Update()
    {




        if (Input.GetKeyDown(KeyCode.A))
        {
            ResetPath();

            System.Diagnostics.Stopwatch watch = new System.Diagnostics.Stopwatch();
            watch.Start();

            _paths = FindPath(transform.position, _target.position);

            watch.Stop();
            UnityEngine.Debug.Log(watch.ElapsedMilliseconds + " ms");
        }

        bool isEnd = IsEndOfPath();
        if (isEnd == true) return;

        _positionToMove.Set(_paths[_pathIndex].x, transform.position.y, _paths[_pathIndex].z);

        Vector3 dir = ReturnDirection();
        _viewComponent.View(dir);

        float distance = Vector3.Distance(transform.position, _positionToMove);
        if(distance < 0.1f)
        {
            if(_paths.Count - 1 < _pathIndex) ResetPath();
            else _pathIndex++;
        }
    }

    private void FixedUpdate()
    {
        bool isEnd = IsEndOfPath();
        if (isEnd == true)
        {
            if(_animator.GetBool("IsRunning") == true)
                _animator.SetBool("IsRunning", false);
        }
        else
        {
            if (_animator.GetBool("IsRunning") == false)
                _animator.SetBool("IsRunning", true);

            Vector3 dir = ReturnDirection();
            _moveComponent.Move(dir);
        }
    }

    private void OnDrawGizmos()
    {
        if (Application.isPlaying == false) return;

        if (_paths == null || _paths.Count == 0) return;

        for (int i = 0; i < _paths.Count; i++)
        {
            Gizmos.DrawCube(_paths[i], new Vector3(1, 1, 1));
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

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
        _moveComponent.Initialize(_moveSpeed);

        _viewComponent = GetComponent<ViewComponent>();
        _viewComponent.Initialize(_rotationSpeed);

        Pathfinder pathfinder = FindObjectOfType<Pathfinder>();
        if (pathfinder == null) return;

        FindPath = pathfinder.FindPath;
    }

    void ResetPath()
    {
        _paths.Clear();
        _pathIndex = 0;
    }

    void ResetPath(List<Vector3> paths)
    {
        int storedCount = 0;
        for (int i = 0; i < paths.Count; i++)
        {
            if (i > paths.Count - 1) return;
            if (_paths[i] == paths[i]) storedCount++;
        }

        if(_pathIndex > storedCount) ResetPath();
    }

    void Update()
    {
        ResetPath(_paths);
        _paths = FindPath(transform.position, _target.position);

        if (_paths.Count == 0 || _paths.Count - 1 < _pathIndex) return;
        _positionToMove.Set(_paths[_pathIndex].x, transform.position.y, _paths[_pathIndex].z);

        float distance = Vector3.Distance(transform.position, _positionToMove);
        if(distance < 0.1f)
        {
            if(_paths.Count - 1 < _pathIndex) ResetPath();
            else _pathIndex++;
        }
    }

    private void FixedUpdate()
    {
        if (_paths.Count == 0)
        {
            if(_animator.GetBool("IsRunning") == true)
                _animator.SetBool("IsRunning", false);
        }
        else
        {
            if (_animator.GetBool("IsRunning") == false)
                _animator.SetBool("IsRunning", true);

            Vector3 dir = (_positionToMove - transform.position).normalized;
            _viewComponent.View(dir);
            _moveComponent.Move(dir);
        }
    }
}

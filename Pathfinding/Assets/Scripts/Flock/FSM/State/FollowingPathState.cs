using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FSM;
using System;

public class FollowingPathState : State
{
    Func<Vector3, Vector3, List<Vector3>> OnFindPathRequested;
    MoveComponent _moveComponent;
    ViewComponent _viewComponent;

    Timer _timer;
    float _minDistanceToPathPoint = 0.3f;
    float _pathFindDelay = 5f; // 딜레이만큼 기다렸다가 다시 길찾기 시작

    List<Vector3> _paths;
    int _pathIndex = 0;

    protected Transform _myTransform;
    protected Vector3 _targetPos;

    public FollowingPathState(MoveComponent moveComponent, ViewComponent viewComponent, Func<Vector3, Vector3, List<Vector3>> onFindPathRequested)
    {
        OnFindPathRequested = onFindPathRequested;
        _moveComponent = moveComponent;
        _viewComponent = viewComponent;

        _myTransform = moveComponent.transform;
        _timer = new Timer();
    }
    public override void CheckStateChange() { }

    public override void OnStateEnter()
    {
        FindPath(); // 처음 길찾기를 시작한다.
        _timer.Start(_pathFindDelay); // 타이머를 시작시킨다.
    }

    public override void OnStateExit()
    {
        _timer.Reset(); // 빠져나갈 때 초기화해준다.
        _pathIndex = 0;
    }

    protected virtual void FindPath()
    {
        _paths = OnFindPathRequested(_myTransform.position, _targetPos);
        _pathIndex = 0;
    }

    Vector3 ReturnDirectionToPathPoint()
    {
       return (_paths[_pathIndex] - _myTransform.position).normalized;
    }

    void MoveAlongPath()
    {
        if (_paths.Count - 1 <= _pathIndex) return; // 끝까지 도달하면 리턴시켜줌

        float distance = Vector3.Distance(_myTransform.position, _paths[_pathIndex]);
        if (distance < _minDistanceToPathPoint) _pathIndex++;

        Vector3 dir = ReturnDirectionToPathPoint();
        _moveComponent.Move(dir);
    }

    void ViewPath()
    {
        Vector3 dir = ReturnDirectionToPathPoint();
        _viewComponent.View(dir);
    }

    public override void OnStateUpdate()
    {
        ViewPath();
        MoveAlongPath();

        _timer.Update();
        if (_timer.IsFinish == false) return;

        FindPath();
        _timer.Reset(); // 초기화 진행
        _timer.Start(_pathFindDelay);
    }
}

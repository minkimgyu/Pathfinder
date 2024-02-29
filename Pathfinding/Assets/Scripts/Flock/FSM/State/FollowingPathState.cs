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
    float _pathFindDelay = 5f; // �����̸�ŭ ��ٷȴٰ� �ٽ� ��ã�� ����

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
        FindPath(); // ó�� ��ã�⸦ �����Ѵ�.
        _timer.Start(_pathFindDelay); // Ÿ�̸Ӹ� ���۽�Ų��.
    }

    public override void OnStateExit()
    {
        _timer.Reset(); // �������� �� �ʱ�ȭ���ش�.
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
        if (_paths.Count - 1 <= _pathIndex) return; // ������ �����ϸ� ���Ͻ�����

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
        _timer.Reset(); // �ʱ�ȭ ����
        _timer.Start(_pathFindDelay);
    }
}

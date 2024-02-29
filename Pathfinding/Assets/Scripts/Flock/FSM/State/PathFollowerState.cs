using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FSM;
using System;
using AI.Flock;

public class PathFollowerState : FollowingPathState
{
    Transform _myTr;
    float _goToFlockFollowerStateDistance;
    Action<FollowComponent.State> OnStateChange;
    Func<Vector3> ReturnLeaderAgentPos;

    public PathFollowerState(Func<Vector3> returnLeaderAgentPos, Action<FollowComponent.State> onStateChange, MoveComponent moveComponent, ViewComponent viewComponent, 
        Func<Vector3, Vector3, List<Vector3>> onFindPathRequested, float goToFlockFollowerStateDistance)
        : base(moveComponent, viewComponent, onFindPathRequested)
    {
        ReturnLeaderAgentPos = returnLeaderAgentPos;
        OnStateChange = onStateChange;
        _myTr = moveComponent.transform;
        _goToFlockFollowerStateDistance = goToFlockFollowerStateDistance;
    }

    public override void CheckStateChange()
    {
        // ������ �Ÿ��� �־��� ���
        float distance = Vector3.Distance(_myTr.position, ReturnLeaderAgentPos());
        if (distance < _goToFlockFollowerStateDistance) OnStateChange(FollowComponent.State.FlockFollower);
    }

    protected override void FindPath()
    {
        // �̰Ŵ� FollowComponent���� �޾Ƽ� ����
        _targetPos = ReturnLeaderAgentPos();
        base.FindPath();
    }
}

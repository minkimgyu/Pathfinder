using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FSM;
using AI.Flock;
using System;

public class FlockFollowerState : State
{
    MoveComponent _moveComponent;
    ViewComponent _viewComponent;
    BaseBehavior[] _baseBehaviors;

    Transform _myTr;

    Func<List<FlockAgent>> OnReturnAgentsRequested;
    Action<FollowComponent.State> OnStateChange;
    Func<Vector3> ReturnLeaderAgentPos;
    float _goToPathFollowerStateDistance;

    public FlockFollowerState(Func<Vector3> returnLeaderAgentPos, Action<FollowComponent.State> onStateChange, MoveComponent moveComponent, ViewComponent viewComponent,
        Func<List<FlockAgent>> onReturnAgentsRequested, BaseBehavior[] baseBehaviors, float goToPathFollowerStateDistance)
    {
        _moveComponent = moveComponent;
        _viewComponent = viewComponent;
        _baseBehaviors = baseBehaviors;

        _myTr = _moveComponent.transform;

        ReturnLeaderAgentPos = returnLeaderAgentPos;
        OnReturnAgentsRequested = onReturnAgentsRequested;
        OnStateChange = onStateChange;
        _goToPathFollowerStateDistance = goToPathFollowerStateDistance;
    }

    public override void CheckStateChange() 
    {
        // 대상과의 거리가 멀어질 경우
        float distance = Vector3.Distance(_myTr.position, ReturnLeaderAgentPos());
        if (distance > _goToPathFollowerStateDistance) OnStateChange(FollowComponent.State.PathFollower);
    }

    public override void OnStateUpdate() 
    {
        Vector3 velocity = Vector3.zero;
        Vector3 targetPos = ReturnLeaderAgentPos();

        List<FlockAgent> agents = OnReturnAgentsRequested();
        for (int i = 0; i < _baseBehaviors.Length; i++)
        {
            _baseBehaviors[i].ResetTargetPos(targetPos);
            velocity += _baseBehaviors[i].ReturnVelocity(agents);
        }

        Vector3 nomalVec = new Vector3(velocity.x, 0, velocity.z).normalized;
        _viewComponent.View(nomalVec);
        _moveComponent.Move(new Vector3(velocity.x, 0, velocity.z) * 0.6f);
    }
}
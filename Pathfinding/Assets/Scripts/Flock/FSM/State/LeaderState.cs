using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FSM;
using System;
using AI;

public class LeaderState : FollowingPathState
{
    Func<List<Transform>> OnReturnTargetsRequested;

    public LeaderState(MoveComponent moveComponent, ViewComponent viewComponent, 
        Func<Vector3, Vector3, List<Vector3>> onFindPathRequested, Func<List<Transform>> onReturnTargetsRequested) 
        : base(moveComponent, viewComponent, onFindPathRequested)
    {
        OnReturnTargetsRequested = onReturnTargetsRequested;
    }
    public override void CheckStateChange() { }

    protected override void FindPath()
    {
        _targetPos = ReturnCloestTargetPos();
        base.FindPath();
    }

    Vector3 ReturnCloestTargetPos()
    {
        List<Transform> targets = OnReturnTargetsRequested();

        if (targets.Count == 0) return _myTransform.position;

        float storedDistance = Vector3.Distance(_myTransform.position, targets[0].position);
        int index = 0;
        for (int i = 0; i < targets.Count; i++)
        {
            float distance = Vector3.Distance(_myTransform.position, targets[i].position);
            if (distance < storedDistance)
            {
                storedDistance = distance;
                index = i;
            }
        }

        return targets[index].position;
    }
}

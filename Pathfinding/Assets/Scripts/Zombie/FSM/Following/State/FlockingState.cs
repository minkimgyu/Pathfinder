using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using FSM;
using AI.Flocking;
using AI;
using BehaviorTree.Nodes;

public class FlockingState : State
{
    List<BaseBehavior> _behaviors;
    Transform _myTransform;
    Action<Vector3> View, Move;

    Func<ITarget, bool> HaveLeaderTargetInNearRange;

    Action<FollowFSM.State> SetState;

    Func<ITarget> ReturnTargetInSight;

    float _speed;

    Vector3 _storedVelocity = Vector3.zero;

    FollowBehavior _followBehavior;

    public FlockingState(Transform myTransform, float speed, Action<FollowFSM.State> SetState, 
        Func<ITarget, bool> HaveLeaderTargetInNearRange, Func<List<IFlockingTarget>> ReturnNearFlockingTargets,
        Func<ITarget> ReturnTargetInSight, Action<Vector3> View, Action<Vector3> Move)
    {
        this.ReturnTargetInSight = ReturnTargetInSight;

        this.SetState = SetState;
        _myTransform = myTransform;
        _speed = speed;

        this.View = View;
        this.Move = Move;
        this.HaveLeaderTargetInNearRange = HaveLeaderTargetInNearRange;

        _behaviors.Add(new AlignmentBehavior(_myTransform, 1, ReturnNearFlockingTargets));
        _behaviors.Add(new CohesionBehavior(_myTransform, 1, ReturnNearFlockingTargets));
        _behaviors.Add(new SeparationBehavior(_myTransform, 1, ReturnNearFlockingTargets));

        _followBehavior = new FollowBehavior(_myTransform, 1);
        _behaviors.Add(_followBehavior);
    }

    public override void CheckStateChange()
    {
        ITarget target = ReturnTargetInSight();
        bool nowHave = HaveLeaderTargetInNearRange(target);
        if (nowHave == false) SetState?.Invoke(FollowFSM.State.Leader);
    }

    public override void OnStateUpdate()
    {
        Vector3 velocity = Vector3.zero;

        IFlockingTarget target = ReturnLeaderTarget(); // ReturnNearTargets 여기에서 따로 분류해서 받기
        if (target == null) return;

        _followBehavior.ResetData(target);

        for (int i = 0; i < _behaviors.Count; i++)
        {
            velocity += _behaviors[i].ReturnVelocity();
        }

        _storedVelocity = Vector3.Lerp(_storedVelocity, velocity, Time.deltaTime);

        Debug.DrawRay(_myTransform.position, _storedVelocity, Color.black);

        View(new Vector3(velocity.x, 0, velocity.z));
        Move(new Vector3(_storedVelocity.x, 0, _storedVelocity.z) * _speed);
    }
}

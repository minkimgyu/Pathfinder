using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using FSM;
using AI;
using BehaviorTree.Nodes;

public class LeaderState : State
{
    Action<FollowFSM.State> SetState;

    Action<Vector3, bool> FollowPath;
    Func<ITarget> ReturnTargetInSight;

    Func<ITarget, bool> HaveLeaderInNearRange;

    public LeaderState(Action<FollowFSM.State> SetState, Func<ITarget, bool> HaveLeaderInNearRange, 
        Func<ITarget> ReturnTargetInSight, Action<Vector3, bool> FollowPath)
    {
        this.SetState = SetState;
        this.HaveLeaderInNearRange = HaveLeaderInNearRange;
        this.ReturnTargetInSight = ReturnTargetInSight;

        this.FollowPath = FollowPath;
    }

    public override void CheckStateChange()
    {
        ITarget target = ReturnTargetInSight();
        bool nowHave = HaveLeaderInNearRange(target);
        if (nowHave == true) SetState?.Invoke(FollowFSM.State.Flocking);
    }

    public override void OnStateUpdate()
    {
        ITarget target = ReturnTargetInSight();
        Vector3 pos = target.ReturnPos();
        FollowPath(pos, true);
    }
}

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

    Func<bool> HaveLeaderTargetInNearRange;

    public LeaderState(Action<FollowFSM.State> SetState, Func<bool> HaveLeaderTargetInNearRange, 
        Func<ITarget> ReturnTargetInSight, Action<Vector3, bool> FollowPath)
    {
        this.SetState = SetState;
        this.HaveLeaderTargetInNearRange = HaveLeaderTargetInNearRange;
        this.ReturnTargetInSight = ReturnTargetInSight;

        this.FollowPath = FollowPath;
    }

    public override void CheckStateChange()
    {
        bool nowHave = HaveLeaderTargetInNearRange();
        if (nowHave == false) SetState?.Invoke(FollowFSM.State.Flocking);
    }

    public override void OnStateUpdate()
    {
        ITarget target = ReturnTargetInSight();
        Vector3 pos = target.ReturnPos();
        FollowPath(pos, true);
    }
}

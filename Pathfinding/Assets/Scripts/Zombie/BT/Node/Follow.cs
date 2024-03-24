using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace BehaviorTree.Utility
{
    public class Follow : Node
    {
        Action<Vector3, bool> FollowPath;
        Func<Transform> ReturnTargetInSight;

        public Follow(Func<Transform> ReturnTargetInSight, Action<Vector3, bool> FollowPath)
        {
            this.FollowPath = FollowPath;
            this.ReturnTargetInSight = ReturnTargetInSight;
        }

        public override NodeState Evaluate()
        {
            Transform target = ReturnTargetInSight();
            FollowPath(target.position, true);
            return NodeState.SUCCESS;
        }
    }
}
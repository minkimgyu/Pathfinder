using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace BehaviorTree.Nodes
{
    public class IsCloseToTarget : Node
    {
        float _range;
        Transform _myTransform;
        Func<ITarget> ReturnTargetInSight;
        Action<string, bool> ResetAnimatorBool;

        public IsCloseToTarget(Transform myTransform, Func<ITarget> ReturnTargetInSight, float range, Action<string, bool> ResetAnimatorBool)
        {
            this.ReturnTargetInSight = ReturnTargetInSight;
            this.ResetAnimatorBool = ResetAnimatorBool;

            _range = range;
            _myTransform = myTransform;
        }
        public override NodeState Evaluate()
        {
            ITarget target = ReturnTargetInSight();
            if (target == null) return NodeState.FAILURE;

            float distanceFromTarget = Vector3.Distance(_myTransform.position, target.ReturnPos());
            if(distanceFromTarget < _range)
            {
                ResetAnimatorBool?.Invoke("IsRunning", false);
                return NodeState.SUCCESS;
            }

            return NodeState.FAILURE;
        }
    }
}
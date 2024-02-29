using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AI.Flock
{
    public class FollowBehavior : BaseBehavior
    {
        Vector3 _pos;

        public override Vector3 ReturnVelocity(List<FlockAgent> nearTr)
        {
            return (_pos - transform.position).normalized * _weight;
        }

        public override void ResetTargetPos(Vector3 pos) { _pos = pos; }
    }
}
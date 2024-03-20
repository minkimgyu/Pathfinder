using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AI.Flock
{
    public class FollowBehavior : BaseBehavior
    {
        [SerializeField] Transform _target;

        public override void Intialize(Transform target)
        {
            _target = target;
        }

        public override Vector3 ReturnVelocity(List<FlockAgent> nearAgents, List<Transform> nearObstacles)
        {
            return (_target.position - transform.position).normalized * _weight;
        }
    }
}
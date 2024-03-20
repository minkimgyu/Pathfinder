using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AI.Flock
{
    abstract public class BaseBehavior : MonoBehaviour
    {
        [SerializeField] protected float _weight = 1;

        public virtual void Intialize(Transform target) { }
        public abstract Vector3 ReturnVelocity(List<FlockAgent> nearAgents, List<Transform> nearObstacles);
    }
}
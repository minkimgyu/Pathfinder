using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AI.Flock
{
    abstract public class BaseBehavior : MonoBehaviour
    {
        [SerializeField] protected float _weight = 1;
        public abstract Vector3 ReturnVelocity(List<FlockAgent> nearTr);
        public virtual void ResetTargetPos(Vector3 pos) { }
    }
}
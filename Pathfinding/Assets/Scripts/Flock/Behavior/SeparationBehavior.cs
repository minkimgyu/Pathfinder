using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AI.Flock
{
    public class SeparationBehavior : BaseBehavior
    {
        public override Vector3 ReturnVelocity(List<FlockAgent> nearTr)
        {
            if (nearTr.Count == 0) return Vector3.zero;

            Vector3 combinedPos = new Vector3();
            for (int i = 0; i < nearTr.Count; i++)
            {
                combinedPos += nearTr[i].transform.position;
            }

            combinedPos /= nearTr.Count;
            return (transform.position - combinedPos).normalized * _weight;
        }
    }
}
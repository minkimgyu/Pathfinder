using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AI.Flock
{
    public class AlignmentBehavior : BaseBehavior
    {
        public override Vector3 ReturnVelocity(List<FlockAgent> nearTr)
        {
            if (nearTr.Count == 0) return transform.forward;

            Vector3 fowardDir = new Vector3();
            for (int i = 0; i < nearTr.Count; i++)
            {
                fowardDir += nearTr[i].transform.forward;
            }

            return fowardDir.normalized * _weight;
        }
    }
}

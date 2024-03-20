using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AI.Flock
{
    public class AlignmentBehavior : BaseBehavior
    {
        public override Vector3 ReturnVelocity(List<FlockAgent> nearAgents, List<Transform> nearObstacles)
        {
            if (nearAgents.Count == 0) return transform.forward * _weight;

            Vector3 fowardDir = new Vector3();
            for (int i = 0; i < nearAgents.Count; i++)
            {
                fowardDir += nearAgents[i].transform.forward;
            }

            return fowardDir.normalized * _weight;
        }
    }
}

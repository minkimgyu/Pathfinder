using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AI.Flock
{
    public class SeparationBehavior : BaseBehavior
    {
        public override Vector3 ReturnVelocity(List<FlockAgent> nearAgents, List<Transform> nearObstacles)
        {
            if (nearAgents.Count == 0) return Vector3.zero;

            Vector3 combinedPos = new Vector3();
            for (int i = 0; i < nearAgents.Count; i++)
            {
                combinedPos += nearAgents[i].transform.position;
            }

            combinedPos /= nearAgents.Count;
            return (transform.position - combinedPos).normalized * _weight;
        }
    }
}
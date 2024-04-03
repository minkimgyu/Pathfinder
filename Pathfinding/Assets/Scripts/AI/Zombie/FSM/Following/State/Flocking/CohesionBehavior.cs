using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace AI.Flocking
{
    public class CohesionBehavior : SurroundingBehavior
    {
        public CohesionBehavior(Transform myTransform, float weight, Func<List<IFlockingTarget>> ReturnNearAgents) : 
            base(myTransform, weight, ReturnNearAgents)
        {
        }

        public override Vector3 ReturnVelocity()
        {
            List<IFlockingTarget> nearAgents = ReturnNearAgents();

            if (nearAgents.Count == 0) return Vector3.zero;

            Vector3 combinedPos = new Vector3();
            for (int i = 0; i < nearAgents.Count; i++)
            {

                combinedPos += nearAgents[i].ReturnPos();
            }

            combinedPos /= nearAgents.Count;
            return (combinedPos - _myTransform.position).normalized * _weight;
        }
    }
}
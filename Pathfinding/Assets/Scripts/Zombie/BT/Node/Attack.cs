using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace BehaviorTree.Utility
{
    public class Attack : Node
    {
        Transform _attackPoint;
        float _attackRadius;
        LayerMask _attackLayer;
        Action<string> ResetAnimatorValue;

        public Attack(Transform attackPoint, float attackRadius, LayerMask attackLayer, Action<string> ResetAnimatorValue)
        {
            _attackPoint = attackPoint;
            _attackRadius = attackRadius;
            _attackLayer = attackLayer;
            this.ResetAnimatorValue = ResetAnimatorValue;
        }
        public override NodeState Evaluate()
        {
            Collider[] colliders = Physics.OverlapSphere(_attackPoint.position, _attackRadius, _attackLayer);
            // ����ؼ� ����
            Debug.Log("Attack");
            ResetAnimatorValue?.Invoke("NowAttack");

            return NodeState.SUCCESS;
        }
    }
}

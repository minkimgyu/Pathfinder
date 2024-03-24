using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FSM;
using System;
using BehaviorTree;
using BehaviorTree.Nodes;
using Tree = BehaviorTree.Tree;
using Node = BehaviorTree.Node;
using AI;

public class TargetFollowingState : State
{
    protected Tree _bt;
    Action<Zombie.State> SetState;
    Action<float> ModifyCaptureRadius;
    float _additiveCaptureRadius;

    Func<bool> IsTargetInSight;

    public TargetFollowingState(Action<Zombie.State> SetState, Action<float> ModifyCaptureRadius, float additiveCaptureRadius,  Func<bool> IsTargetInSight, Transform myTrasform, 
        Func<Transform> ReturnTargetInSight, float canAttackRange, float delayDuration, Transform attackPoint, float attackRadius, 
        LayerMask attackLayer, Action<Vector3, bool> FollowPath, Action<string> ResetAnimatorTrigger, Action<string, bool> ResetAnimatorBool)
    {
        this.SetState = SetState;
        this.ModifyCaptureRadius = ModifyCaptureRadius;
        _additiveCaptureRadius = additiveCaptureRadius;

        this.IsTargetInSight = IsTargetInSight;

        _bt = new Tree();
        List<Node> _childNodes;
        _childNodes = new List<Node>()
        {
            new Selector
            (
                new List<Node>()
                {
                     new Sequencer
                     (
                         new List<Node>()
                         {
                            new IsCloseToTarget(myTrasform, ReturnTargetInSight, canAttackRange, ResetAnimatorBool),
                            new Sequencer
                            (
                                new List<Node>()
                                {
                                    new WaitForNextAttack(delayDuration),
                                    new Attack(attackPoint, attackRadius, attackLayer, ResetAnimatorTrigger),
                                    // Wander�� �̺�Ʈ�� ������ ������� ������ �����ش�.
                                }
                            ),
                         }
                     ),
                     new Follow(ReturnTargetInSight, FollowPath)
                }
            )
        };

        Node rootNode = new Selector(_childNodes);
        _bt.SetUp(rootNode);
    }

    public override void CheckStateChange()
    {
        bool isInSight = IsTargetInSight();
        if (isInSight == true) return;

        SetState?.Invoke(Zombie.State.Idle);
    }

    public override void OnStateEnter()
    {
        ModifyCaptureRadius?.Invoke(_additiveCaptureRadius);
        Debug.Log("FollowState");
    }

    public override void OnStateUpdate()
    {
        _bt.OnUpdate();
    }

    // ���⼭ delay �����غ���
    public override void OnStateExit()
    {
        ModifyCaptureRadius?.Invoke(-_additiveCaptureRadius);
        _bt.OnDisable();
    }
}

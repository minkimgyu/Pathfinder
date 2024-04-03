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

    FollowFSM _followFSM;

    public TargetFollowingState(TargetFollowingStateParameter parameter)
    {
        SetState = parameter.SetState;
        ModifyCaptureRadius = parameter.ModifyCaptureRadius;
        IsTargetInSight = parameter.IsTargetInSight;

        _additiveCaptureRadius = parameter.additiveCaptureRadius;
        _followFSM = new FollowFSM(parameter.ReturnTargetInSight, parameter.FollowPath);

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
                            new IsCloseToTarget(parameter.myTrasform, parameter.ReturnTargetInSight, parameter.canAttackRange, parameter.ResetAnimatorBool),
                            new Sequencer
                            (
                                new List<Node>()
                                {
                                    new WaitForNextAttack(parameter.delayDuration),
                                    new Attack(parameter.attackPoint, parameter.attackRadius, parameter.attackLayer, parameter.ResetAnimatorTrigger),
                                    // Wander에 이벤트를 보내는 방식으로 방향을 돌려준다.
                                }
                            ),
                         }
                     ),
                     _followFSM
                }
            )
        };

        Node rootNode = new Selector(_childNodes);
        _bt.SetUp(rootNode);
    }

    public FollowFSM.State ReturnFollowState()
    {
        return _followFSM.ReturnState();
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

    // 여기서 delay 리셋해보자
    public override void OnStateExit()
    {
        ModifyCaptureRadius?.Invoke(-_additiveCaptureRadius);
        _bt.OnDisable();
    }
}

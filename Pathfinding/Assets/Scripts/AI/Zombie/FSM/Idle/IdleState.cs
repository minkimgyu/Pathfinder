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

public class IdleState : State
{
    protected Tree _bt;
    Action<Zombie.State> SetState;
    Func<bool> IsTargetInSight;

    public IdleState(IdleStateParameter parameter)
    {
        SetState = parameter.SetState;
        IsTargetInSight = parameter.IsTargetInSight;

        WanderingFSMParameter wanderingFSMParameter = new WanderingFSMParameter(parameter.myTrasform, parameter.wanderOffset, 
            parameter.View, parameter.Stop, parameter.FollowPath, parameter.ReturnNodePos);

        WanderingFSM wanderingFSM = new WanderingFSM(wanderingFSMParameter);
        ChangeToRandomState changeState = new ChangeToRandomState(wanderingFSM.FSM.SetState);

        _bt = new Tree();
        List<Node> _childNodes;
        _childNodes = new List<Node>()
        {
            new Sequencer
            (
                new List<Node>()
                {
                    new ChangeAngleOfSight(parameter.captureTransform, parameter.angleOffset, parameter.angleChangeAmount),
                    wanderingFSM,

                    new Sequencer
                    (
                        new List<Node>()
                        {
                            new WaitForStateChange(parameter.stateChangeDelay),
                            changeState,
                            // Wander�� �̺�Ʈ�� ������ ������� ������ �����ش�.
                        }
                    ),
                }
            )
        };

        Node rootNode = new Selector(_childNodes);
        _bt.SetUp(rootNode);
    }

    // �̺�Ʈ�� �̿��ؼ� �ޱ�
    // ���� �Ҹ��� �����ߴٸ� ���� Ż��
    public override void OnReceiveNoise()
    {
        SetState?.Invoke(Zombie.State.NoiseTracking);
    }

    public override void CheckStateChange()
    {
        bool isInSight = IsTargetInSight();
        if (isInSight == false) return;

        SetState?.Invoke(Zombie.State.TargetFollowing);
    }

    public override void OnStateEnter()
    {
        //Debug.Log("IdleState");
    }

    public override void OnStateUpdate()
    {
        _bt.OnUpdate();
    }

    // ���⼭ delay �����غ���
    public override void OnStateExit()
    {
        _bt.OnDisable();
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FSM;
using System;
using BehaviorTree;
using BehaviorTree.Utility;
using Tree = BehaviorTree.Tree;
using Node = BehaviorTree.Node;
using AI;

public class IdleState : State
{
    protected Tree _bt;
    Action<Zombie.State> SetState;
    Func<bool> IsTargetInSight;

    public IdleState(Action<Zombie.State> SetState, Transform captureTransform, Func<bool> IsTargetInSight, float angleOffset, float angleChangeAmount, int wanderOffset,
        float stateChangeDelay, Transform myTrasform, Func<Vector3, Vector3> ReturnNodePos, Action<Vector3, bool> FollowPath, Action<Vector3> View)
    {
        this.SetState = SetState;
        this.IsTargetInSight = IsTargetInSight;

        WanderingFSM wanderingFSM = new WanderingFSM(ReturnNodePos, View, FollowPath, myTrasform, wanderOffset);
        ChangeToRandomState changeState = new ChangeToRandomState(wanderingFSM.FSM.SetState);

        _bt = new Tree();
        List<Node> _childNodes;
        _childNodes = new List<Node>()
        {
            new Sequencer
            (
                new List<Node>()
                {
                    new ChangeAngleOfSight(captureTransform, angleOffset, angleChangeAmount),
                    wanderingFSM,

                    new Sequencer
                    (
                        new List<Node>()
                        {
                            new WaitForNextStateChange(stateChangeDelay),
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
        Debug.Log("IdleState");
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

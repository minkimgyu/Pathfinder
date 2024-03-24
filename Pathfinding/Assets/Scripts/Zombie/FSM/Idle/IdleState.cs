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

    public IdleState(Action<Zombie.State> SetState, Transform captureTransform, Func<bool> IsTargetInSight, float angleOffset, float angleChangeAmount, 
        int wanderOffset, float stateChangeDelay, Transform myTrasform, Func<Vector3, Vector3> ReturnNodePos, Action<Vector3, bool> FollowPath, Action<Vector3> View)
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
                            new WaitForStateChange(stateChangeDelay),
                            changeState,
                            // Wander에 이벤트를 보내는 방식으로 방향을 돌려준다.
                        }
                    ),
                }
            )
        };

        Node rootNode = new Selector(_childNodes);
        _bt.SetUp(rootNode);
    }

    // 이벤트를 이용해서 받기
    // 만약 소리를 감지했다면 루프 탈출
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

    // 여기서 delay 리셋해보자
    public override void OnStateExit()
    {
        _bt.OnDisable();
    }
}

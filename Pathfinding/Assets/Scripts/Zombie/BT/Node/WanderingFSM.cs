using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FSM;
using BehaviorTree;
using System;

public class WanderingFSM : Node
{
    // FSM으로 처리
    public enum State
    {
        Move,
        Stop,
        Rotate
    }

    StateMachine<State> _fsm;
    public StateMachine<State> FSM { get { return _fsm; } }

    public WanderingFSM(Func<Vector3, Vector3> ReturnNodePos, Action<Vector3> View, Action<Vector3, bool> FollowPath, Transform myTrasform, int wanderOffset)
    {
        _fsm = new StateMachine<State>();
        Dictionary<State, BaseState> states = new Dictionary<State, BaseState>()
        {
            {State.Move, new MoveState(ReturnNodePos, FollowPath, myTrasform, wanderOffset) },
            {State.Stop, new StopState() },
            {State.Rotate, new RotateState(View, myTrasform) }
        };

        _fsm.Initialize(states);
        _fsm.SetState(State.Move);
    }

    public override NodeState Evaluate()
    {
        _fsm.OnUpdate();
        return NodeState.SUCCESS;
    }
}

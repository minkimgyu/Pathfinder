using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FSM;
using System;

namespace BehaviorTree.Nodes
{
    public class WanderingFSM : Node
    {
        // FSM���� ó��
        public enum State
        {
            Move,
            Stop,
            Rotate
        }

        StateMachine<State> _fsm;
        public StateMachine<State> FSM { get { return _fsm; } }

        public WanderingFSM(WanderingFSMParameter parameter)
        {
            _fsm = new StateMachine<State>();
            Dictionary<State, BaseState> states = new Dictionary<State, BaseState>()
            {
                {State.Move, new MoveState(parameter.ReturnNodePos, parameter.FollowPath, parameter.myTrasform, parameter.wanderOffset) },
                {State.Stop, new StopState(parameter.Stop) },
                {State.Rotate, new RotateState(parameter.View, parameter.Stop, parameter.myTrasform) }
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
}
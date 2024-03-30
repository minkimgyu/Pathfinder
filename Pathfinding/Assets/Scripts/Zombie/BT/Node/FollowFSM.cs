using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using FSM;

namespace BehaviorTree.Nodes
{
    public class FollowFSM : Node
    {
        public enum State
        {
            Leader,
            Flocking,
        }

        StateMachine<State> _fsm = new StateMachine<State>();

        public FollowFSM(Func<Transform> ReturnTargetInSight, Action<Vector3, bool> FollowPath)
        {
            Dictionary<State, BaseState> states = new Dictionary<State, BaseState>
            {
                {State.Leader, new LeaderState(ReturnTargetInSight, FollowPath) },
                {State.Flocking, new FlockingState() }
            };

            _fsm.Initialize(states);
            _fsm.SetState(State.Leader);
        }

        public State ReturnState()
        {
            return _fsm.ReturnCurrentState();
        }

        public override NodeState Evaluate()
        {
            _fsm.OnUpdate();
            return NodeState.SUCCESS;
        }
    }
}
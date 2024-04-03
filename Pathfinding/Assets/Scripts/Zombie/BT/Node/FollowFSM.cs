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

        public FollowFSM(Transform myTransform, float flockingSpeed,  Func<ITarget, bool> HaveLeaderInNearRange, 
            Func<ITarget> ReturnTargetInSight, Action<Vector3, bool> FollowPath,
            Func<List<IFlockingTarget>> ReturnNearFlockingTargets, Action<Vector3> View, Action<Vector3> Move)
        {
            Dictionary<State, BaseState> states = new Dictionary<State, BaseState>
            {
                {State.Leader, new LeaderState(SetState, HaveLeaderInNearRange, ReturnTargetInSight, FollowPath) },
                {State.Flocking, new FlockingState(myTransform, flockingSpeed, SetState, HaveLeaderInNearRange,
                ReturnNearFlockingTargets, ReturnTargetInSight, View, Move) }
            };

            _fsm.Initialize(states);
            _fsm.SetState(State.Leader);
        }

        void SetState(State state)
        {
            _fsm.SetState(state);
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
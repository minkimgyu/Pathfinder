using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FSM;
using System;

namespace AI.Flock
{
    public class FollowComponent : MonoBehaviour
    {
        // FSM으로 처리
        public enum State
        { 
            Leader,
            PathFollower,
            FlockFollower,
        }

        [SerializeField] SkinnedMeshRenderer _renderer;
        [SerializeField] Material _material1;
        [SerializeField] Material _material2;
        [SerializeField] Material _material3;

        FlockAgent _leaderAgent;

        StateMachine<State> _fsm;
        public StateMachine<State> FSM { get { return _fsm; } }

        public void Initialize(MoveComponent moveComponent, ViewComponent viewComponent, Func<Vector3, Vector3, List<Vector3>> OnFindPathRequested, 
            Func<List<Transform>> OnReturnTargetsRequested, Func<List<FlockAgent>> OnReturnAgentsRequested, BaseBehavior[] baseBehaviors, float goToPathFollowerStateDistance, float goToFlockFollowerStateDistance)
        {
            _fsm = new StateMachine<State>();
            Dictionary<State, BaseState> states = new Dictionary<State, BaseState>()
            {
                {State.Leader, new LeaderState(moveComponent, viewComponent, OnFindPathRequested, OnReturnTargetsRequested) },
                {State.PathFollower, new PathFollowerState(ReturnLeaderAgentPos, OnStateChange, moveComponent, viewComponent, OnFindPathRequested, goToFlockFollowerStateDistance) },
                {State.FlockFollower, new FlockFollowerState(ReturnLeaderAgentPos, OnStateChange, moveComponent, viewComponent, OnReturnAgentsRequested, baseBehaviors, goToPathFollowerStateDistance) }
            };

            _fsm.Initialize(states);
            OnStateChange(State.FlockFollower);
        }

        public bool IsLeader() { return _fsm.ReturnCurrentState() == State.Leader; }

        void OnStateChange(State stateName)
        {
            if(stateName == State.Leader)
            {
                _renderer.material = _material1;
            }
            else if (stateName == State.PathFollower)
            {
                _renderer.material = _material2;
            }
            else
            {
                _renderer.material = _material3;
            }

            _fsm.SetState(stateName);
        }

        public void OnResetLeader()
        {
            print("OnResetLeader");
            OnStateChange(State.Leader);
        }

        // 리더가 바뀌면 재지정해준다.
        public void ResetLeader(FlockAgent agent) { _leaderAgent = agent; }

        // 리더가 바뀌면 재지정해준다.
        Vector3 ReturnLeaderAgentPos() { return _leaderAgent.transform.position; }

        public void OnUpdate()
        {
            _fsm.OnUpdate();
        }
    }
}

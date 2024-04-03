using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FSM;
using Grid;
using Grid.Pathfinder;
using BehaviorTree.Nodes;

namespace AI
{
    public class Zombie : MonoBehaviour, IFlockingTarget
    {
        [SerializeField] Animator _animator;
        [SerializeField] ViewCaptureComponent _captureComponent;
        [SerializeField] float _angleOffset;
        [SerializeField] float _angleChangeAmount;
        [SerializeField] float _stateChangeDelay;

        [SerializeField] float _moveSpeed = 3;
        [SerializeField] float _viewSpeed = 5;
        [SerializeField] int _wanderOffset = 7;

        [SerializeField] float _targetCaptureRadius = 8;
        [SerializeField] float _targetCaptureAdditiveRadius = 1f;
        [SerializeField] float _targetCaptureAngle = 90;

        [SerializeField] float _noiseCaptureRadius = 11;
        [SerializeField] int _maxNoiseQueueSize = 3;

        [SerializeField] Transform _attackPoint;
        [SerializeField] float _attackRadius = 1f;
        [SerializeField] float _canAttackRange = 1.5f;
        [SerializeField] LayerMask _attackLayer;

        [SerializeField] float _delayForNextAttack = 3;

        [SerializeField] float _pathFindDelay = 0.5f;

        TargetFollowingState _targetFollowingState;

        public enum State
        {
            Idle,
            TargetFollowing,
            NoiseTracking
        }

        StateMachine<State> _fsm = new StateMachine<State>();

        public TargetType MyType { get; set; }

        private void Start() => Invoke("Initialize", 1);

        void Initialize()
        {
            GridManager gridManager = FindObjectOfType<GridManager>();
            Pathfinder pathfinder = FindObjectOfType<Pathfinder>();

            _captureComponent = GetComponentInChildren<ViewCaptureComponent>();
            _captureComponent.Initialize(_targetCaptureRadius, _targetCaptureAngle);

            NoiseListener noiseListener = GetComponentInChildren<NoiseListener>();
            noiseListener.Initialize(_noiseCaptureRadius, _maxNoiseQueueSize, OnNoiseReceived);

            MoveComponent moveComponent = GetComponent<MoveComponent>();
            moveComponent.Initialize(_moveSpeed);
            ViewComponent viewComponent = GetComponent<ViewComponent>();
            viewComponent.Initialize(_viewSpeed);

            RouteTrackingComponent routeTrackingComponent = GetComponent<RouteTrackingComponent>();
            routeTrackingComponent.Initialize(_pathFindDelay, moveComponent.Move, viewComponent.View, pathfinder.FindPath, ResetAnimatorBool);

            IdleStateParameter idleStateParameter = new IdleStateParameter(
                 _angleOffset, _angleChangeAmount, _wanderOffset, _stateChangeDelay, _captureComponent.transform, transform, SetState,
                 routeTrackingComponent.FollowPath, viewComponent.View, _captureComponent.IsTargetInSight, gridManager.ReturnNodePos);

            NoiseTrackingStateParameter noiseTrackingStateParameter = new NoiseTrackingStateParameter(
                SetState, noiseListener.ClearAllNoise, routeTrackingComponent.FollowPath, _captureComponent.IsTargetInSight,
                noiseListener.IsQueueEmpty, noiseListener.ReturnFrontNoise, routeTrackingComponent.IsFollowingFinish);

            TargetFollowingStateParameter targetFollowingStateParameter = new TargetFollowingStateParameter(
                _targetCaptureAdditiveRadius, _canAttackRange, _delayForNextAttack, _canAttackRange, _attackLayer,
                _attackPoint, transform, SetState, _captureComponent.ModifyCaptureRadius, routeTrackingComponent.FollowPath,
                ResetAnimatorTrigger, ResetAnimatorBool, _captureComponent.IsTargetInSight, _captureComponent.ReturnTargetInSight);

            _targetFollowingState = new TargetFollowingState(targetFollowingStateParameter);

            Dictionary<State, BaseState> states = new Dictionary<State, BaseState>
            {
                {State.Idle, new IdleState(idleStateParameter) },
                {State.NoiseTracking, new NoiseTrackingState(noiseTrackingStateParameter) },
                {State.TargetFollowing, _targetFollowingState }
            };

            _fsm.Initialize(states);
            SetState(State.Idle);
        }

        void ResetAnimatorTrigger(string triggerName) { _animator.SetTrigger(triggerName); }
        void ResetAnimatorBool(string boolName, bool value) { _animator.SetBool(boolName, value); }

        void SetState(State state)
        {
            _fsm.SetState(state);
        }
       
        void OnNoiseReceived()
        {
            _fsm.OnNoiseReceived();
        }

        private void Update()
        {
            if (_fsm == null) return;
            _fsm.OnUpdate();
        }

        public void InitializeType(TargetType type) { MyType = type; }

        public bool IsSameType(TargetType type) { return type == MyType; }

        public Transform ReturnTransform() { return transform; }
        public Vector3 ReturnPos() { return transform.position; }

        public bool IsLeader(ITarget target)
        {
            if (_fsm.ReturnCurrentState() != State.TargetFollowing) return false;

            FollowFSM.State state = _targetFollowingState.ReturnFollowState();
            ITarget capturedTarget = _captureComponent.ReturnTargetInSight();

            return state == FollowFSM.State.Leader && capturedTarget == target;
        }
    }
}
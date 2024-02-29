using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace AI.Flock
{
    public class FlockAgent : MonoBehaviour
    {
        [SerializeField] BaseBehavior[] _baseBehaviors;
        FollowComponent _followComponent;
        Animator _animator;

        public bool IsLeader { get { return _followComponent.IsLeader(); } }

        public void Initialize(Func<Vector3, Vector3, List<Vector3>> OnFindPathRequested, Func<List<Transform>> OnReturnTargetsRequested, 
            float captureRadius, float agentMoveSpeed, float agentViewSpeed, float goToPathFollowerStateDistance, float goToFlockFollowerStateDistance)
        {
            FlockCaptureComponent captureComponent = GetComponentInChildren<FlockCaptureComponent>();
            captureComponent.Initialize(captureRadius);

            MoveComponent moveComponent = GetComponent<MoveComponent>();
            moveComponent.Initialize(agentMoveSpeed);

            ViewComponent viewComponent = GetComponent<ViewComponent>();
            viewComponent.Initialize(agentViewSpeed);

            _followComponent = GetComponent<FollowComponent>();
            _followComponent.Initialize(moveComponent, viewComponent, OnFindPathRequested, OnReturnTargetsRequested, 
                captureComponent.ReturnNearComponents, _baseBehaviors, goToPathFollowerStateDistance, goToFlockFollowerStateDistance);

            _animator = GetComponent<Animator>();
        }

        public void OnResetLeader()
        {
            _followComponent.OnResetLeader();
        }

        public void ResetLeader(FlockAgent agent)
        {
            _followComponent.ResetLeader(agent);
        }

        private void Update()
        {
            _followComponent.OnUpdate();

            //_followComponent.OnResetLeader();

            //_velocity.Set(0, 0, 0); // √ ±‚»≠

            //if (_velocity == Vector3.zero)
            //{
            //    if (_animator.GetBool("IsRunning") == true)
            //        _animator.SetBool("IsRunning", false);
            //}
            //else
            //{
            //    if (_animator.GetBool("IsRunning") == false)
            //        _animator.SetBool("IsRunning", true);
            //}
        }

        private void FixedUpdate()
        {
            _followComponent.OnFixedUpdate();
        }

        //private void OnDrawGizmos()
        //{
        //    if (Application.isPlaying == false || _velocity == null) return;

        //    Gizmos.color = Color.blue;
        //    Gizmos.DrawLine(transform.position, transform.position + _velocity.normalized);
        //}
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace AI.Flock
{
    public class FlockAgent : MonoBehaviour
    {
        [SerializeField] BaseBehavior[] _baseBehaviors;

        FlockCaptureComponent _flockCaptureComponent;
        ObstacleCaptureComponent _obstacleCaptureComponent;
        ViewComponent _viewComponent;
        MoveComponent _moveComponent;

        [SerializeField] float _speed;

        Animator _animator;

        Vector3 _storedVelocity = Vector3.zero;

        public void Initialize(Transform target, float captureRadius, float agentMoveSpeed, float agentViewSpeed)
        {
            _flockCaptureComponent = GetComponentInChildren<FlockCaptureComponent>();
            _flockCaptureComponent.Initialize(captureRadius);

            _obstacleCaptureComponent = GetComponentInChildren<ObstacleCaptureComponent>();
            _obstacleCaptureComponent.Initialize("Obstacle");

            _moveComponent = GetComponent<MoveComponent>();
            //_moveComponent.Initialize(agentMoveSpeed);

            _viewComponent = GetComponent<ViewComponent>();
            _viewComponent.Initialize(agentViewSpeed);

            _animator = GetComponent<Animator>();

            for (int i = 0; i < _baseBehaviors.Length; i++)
            {
                _baseBehaviors[i].Intialize(target);
            }
        }

        private void Update()
        {
            Vector3 velocity = Vector3.zero;

            List<FlockAgent> agents = _flockCaptureComponent.ReturnNearComponents();
            List<Transform> obstacles = _obstacleCaptureComponent.ReturnNearComponents();

            for (int i = 0; i < _baseBehaviors.Length; i++)
            {
                velocity += _baseBehaviors[i].ReturnVelocity(agents, obstacles);
            }

            if (velocity == Vector3.zero)
            {
                if (_animator.GetBool("IsRunning") == true)
                    _animator.SetBool("IsRunning", false);
            }
            else
            {
                if (_animator.GetBool("IsRunning") == false)
                    _animator.SetBool("IsRunning", true);
            }

            _storedVelocity = Vector3.Lerp(_storedVelocity, velocity, Time.deltaTime);

            Debug.DrawRay(transform.position, _storedVelocity, Color.black);

            _viewComponent.View(new Vector3(velocity.x, 0, velocity.z));
            _moveComponent.Move(new Vector3(_storedVelocity.x, 0, _storedVelocity.z) * _speed);
        }
    }
}
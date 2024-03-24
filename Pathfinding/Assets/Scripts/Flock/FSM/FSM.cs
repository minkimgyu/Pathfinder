using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AI.Flock;

namespace FSM
{
    abstract public class State : BaseState
    {
        public override void CheckStateChange() { }

        public override void OnReceiveNoise() { }


        public override void OnStateEnter() { }

        public override void OnStateUpdate() { }

        public override void OnStateExit() { }
    }

    abstract public class BaseState
    {
        public abstract void CheckStateChange();

        public abstract void OnReceiveNoise();


        public abstract void OnStateEnter();

        public abstract void OnStateUpdate();

        public abstract void OnStateExit();
    }

    public class StateMachine<T>
    {
        Dictionary<T, BaseState> _stateDictionary = new Dictionary<T, BaseState>();

        T _currentStateName;
        T _previousStateName;
        BaseState _currentState;
        BaseState _previousState;

        public void Initialize(Dictionary<T, BaseState> stateDictionary)
        {
            _currentState = null;
            _previousState = null;

            _stateDictionary = stateDictionary;
        }

        public T ReturnCurrentState() { return _currentStateName; }

        public void OnUpdate()
        {
            if (_currentState == null) return;
            _currentState.OnStateUpdate();
            _currentState.CheckStateChange();
        }

        public void OnNoiseReceived()
        {
            if (_currentState == null) return;
            _currentState.OnReceiveNoise();
        }

        public bool RevertToPreviousState()
        {
            T stateName = _currentStateName;
            _currentStateName = _previousStateName;
            _previousStateName = stateName;
            return ChangeState(_previousState);
        }

        #region SetState

        public bool SetState(T stateName)
        {
            _previousStateName = _currentStateName;
            _currentStateName = stateName;
            return ChangeState(_stateDictionary[stateName]);
        }

        #endregion


        #region ChangeState

        bool ChangeState(BaseState state)
        {
            if (_stateDictionary.ContainsValue(state) == false) return false;

            if (_currentState == state) 
            {
                return false;
            }

            if (_currentState != null)
                _currentState.OnStateExit();

            _previousState = _currentState;

            _currentState = state;


            if (_currentState != null)
            {
                _currentState.OnStateEnter();
            }

            return true;
        }

        #endregion
    }
}
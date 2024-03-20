using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FSM;

public class Zombie : MonoBehaviour
{
    public enum ActionState
    {
        Ready,
        Neutral,
        Active
    }

    protected StateMachine<ActionState> _fsm;
    public StateMachine<ActionState> FSM { get { return _fsm; } }

    // 여기에 FSM 넣어서 Ready, Active 여부 확인해주기
    protected Tree _bt;
    public Tree BT { get { return _bt; } }

    protected override void InitializeComponent()
    {
        base.InitializeComponent();
        _fsm = new StateMachine<ActionState>();

        _bt = new Tree();
        InitializeBT(); // 여기서 초기화 진행
    }

    // 이건 가장 마지막에 실행시켜주자
    protected void InitializeFSM(Dictionary<ActionState, BaseState> states, ActionState startState)
    {
        _fsm.Initialize(states);
        _fsm.SetState(startState);
    }

    public override void GetDamage(float damage)
    {
        base.GetDamage(damage);
        _fsm.OnActive();
    }

    // Update is called once per frame
    protected virtual void Update() => _fsm.OnUpdate(); // 업데이트 적용

    protected abstract void InitializeBT();
}
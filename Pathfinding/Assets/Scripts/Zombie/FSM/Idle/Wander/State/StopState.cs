using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FSM;

public class StopState : State
{
    public override void OnStateEnter()
    {
        Debug.Log("StopState");
    }
}

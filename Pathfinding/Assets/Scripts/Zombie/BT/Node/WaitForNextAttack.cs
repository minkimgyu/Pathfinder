using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviorTree.Utility
{
    public class WaitForNextAttack : Node
    {
        StopwatchTimer _stopwatchTimer;
        float _delayDuration;

        public WaitForNextAttack(float delayDuration)
        {
            _stopwatchTimer = new StopwatchTimer();
            _delayDuration = delayDuration;
        }

        public override NodeState Evaluate()
        {
            switch (_stopwatchTimer.CurrentState)
            {
                case StopwatchTimer.State.Ready:

                    _stopwatchTimer.Start(_delayDuration);
                    return NodeState.SUCCESS;
                case StopwatchTimer.State.Running:

                    return NodeState.RUNNING;
                case StopwatchTimer.State.Finish:

                    _stopwatchTimer.Reset();
                    _stopwatchTimer.Start(_delayDuration);
                    return NodeState.SUCCESS;
            }

            return NodeState.FAILURE;
        }

        public override void OnDisableRequested()
        {
            _stopwatchTimer.Reset();
        }
    }
}
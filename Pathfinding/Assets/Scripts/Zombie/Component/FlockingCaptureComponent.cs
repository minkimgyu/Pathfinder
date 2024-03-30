using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlockingCaptureComponent : MovableTargetCaptureComponent<IFlockingTarget>
{
    IFlockingTarget _flockingTarget;

    public bool HaveLeaderTargetInNearRange()
    {
        for (int i = 0; i < _capturedTargets.Count; i++)
        {
            if (_capturedTargets[i].IsLeaderState())
            {
                _flockingTarget = _capturedTargets[i];
            }
            return true;

        }

        return false;
    }

    public List<IFlockingTarget> ReturnNearTargets() { return _capturedTargets; }

    public IFlockingTarget ReturnLeaderTarget() { return _flockingTarget; }
}
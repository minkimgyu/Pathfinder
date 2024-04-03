using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using AI;

public struct TargetFollowingStateParameter
{
    public TargetFollowingStateParameter(
        float additiveCaptureRadius,
        float canAttackRange,
        float delayDuration,
        float attackRadius,
        LayerMask attackLayer,
        Transform attackPoint,
        Transform myTrasform,

        Action<Zombie.State> SetState,
        Action<float> ModifyCaptureRadius,
        Action<Vector3, bool> FollowPath,
        Action<string> ResetAnimatorTrigger,
        Action<string, bool> ResetAnimatorBool,

        Func<bool> IsTargetInSight,
        Func<ITarget> ReturnTargetInSight
        )
    {
        this.additiveCaptureRadius = additiveCaptureRadius;
        this.canAttackRange = canAttackRange;
        this.delayDuration = delayDuration;
        this.attackRadius = attackRadius;
        this.attackLayer = attackLayer;
        this.attackPoint = attackPoint;
        this.myTrasform = myTrasform;

        this.SetState = SetState;
        this.ModifyCaptureRadius = ModifyCaptureRadius;
        this.FollowPath = FollowPath;
        this.ResetAnimatorTrigger = ResetAnimatorTrigger;
        this.ResetAnimatorBool = ResetAnimatorBool;

        this.IsTargetInSight = IsTargetInSight;
        this.ReturnTargetInSight = ReturnTargetInSight;
    }

    public float additiveCaptureRadius { get; }
    public float canAttackRange { get; }
    public float delayDuration { get; }
    public float attackRadius { get; }
    public LayerMask attackLayer { get; }
    public Transform attackPoint { get; }
    public Transform myTrasform { get; }

    public Action<Zombie.State> SetState { get; }
    public Action<float> ModifyCaptureRadius { get; }
    public Action<Vector3, bool> FollowPath { get; }
    public Action<string> ResetAnimatorTrigger { get; }
    public Action<string, bool> ResetAnimatorBool { get; }

    public Func<bool> IsTargetInSight { get; }
    public Func<ITarget> ReturnTargetInSight { get; }
}
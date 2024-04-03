using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TargetType
{
    Sound,
    Human,
    Zombie
}

public interface ITarget
{
    TargetType MyType { get; set; }

    void InitializeType(TargetType type);

    bool IsSameType(TargetType type);

    Transform ReturnTransform();
    Vector3 ReturnPos();
}

public interface IFlockingTarget : ITarget
{
    /// <summary>
    /// FollowFSM�� State�� Leader�� ���
    /// </summary>
    bool IsLeader(ITarget target);
}
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
    Transform ReturnTransform();
    Vector3 ReturnPos();
}

public interface IFlockingTarget : ITarget
{
    /// <summary>
    /// FollowFSM�� State�� Leader�� ���
    /// </summary>
    bool IsLeader();

    /// <summary>
    ///  ���� ��ǥ�� ������ ���� ��
    /// </summary>
    bool HasSameTarget(ITarget target);
}
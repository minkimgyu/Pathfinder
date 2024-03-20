using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveComponent : MonoBehaviour
{
    Rigidbody _rigid;
    float _speed;

    public void Initialize(float speed)
    {
        _speed = speed;
        _rigid = GetComponent<Rigidbody>();
    }

    public Vector3 ReturnVelocity()
    {
        return _rigid.velocity;
    }

    public void Move(Vector3 dir)
    {
        _rigid.AddForce(dir * _speed, ForceMode.Force);
    }
}
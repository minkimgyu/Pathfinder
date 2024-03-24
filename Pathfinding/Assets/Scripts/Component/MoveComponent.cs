using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

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
        //_rigid.MovePosition(_rigid.position + dir * _speed * Time.deltaTime);
        _rigid.velocity = dir * _speed;
    }
}
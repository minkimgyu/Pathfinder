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

    public void Move(Vector3 dir)
    {
        _rigid.MovePosition(_rigid.position + dir * _speed * Time.deltaTime);
    }
}
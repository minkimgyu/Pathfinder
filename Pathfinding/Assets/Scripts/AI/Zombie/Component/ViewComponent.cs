using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ViewComponent : MonoBehaviour
{
    float _speed;

    public void Initialize(float speed)
    {
        _speed = speed;
    }

    public void View(Vector3 dir)
    {
        dir.Set(dir.x, 0, dir.z);
        transform.forward = Vector3.Lerp(transform.forward, dir, Time.smoothDeltaTime * _speed);
    }
}

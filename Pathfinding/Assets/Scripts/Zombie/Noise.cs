using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Noise : MonoBehaviour
{
    BaseDrawer _sphereDrawer;
    SphereCollider _sphereCollider;

    public void Initialize(float radius, float disableTime)
    {
        _sphereDrawer = GetComponent<BaseDrawer>();
        _sphereCollider = GetComponent<SphereCollider>();

        _sphereDrawer.ResetData(radius);
        _sphereCollider.radius = radius;

        Destroy(gameObject, disableTime);
    }
}

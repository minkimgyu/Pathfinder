using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CircleCaptureComponent<T> : CaptureComponent<T>
{
    CapsuleCollider _captureCollider;

    public override void Initialize(float captureRadius)
    {
        base.Initialize(captureRadius);
        _captureCollider = GetComponent<CapsuleCollider>();
        _captureCollider.radius = captureRadius;
    }
}

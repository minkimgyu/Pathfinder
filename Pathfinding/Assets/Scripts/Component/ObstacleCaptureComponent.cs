using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleCaptureComponent : CaptureComponent<Transform>
{
    public override void Initialize(string captureTag)
    {
        base.Initialize(captureTag);
        _captureTag = captureTag;
    }
}

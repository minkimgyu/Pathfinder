using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class NoiseListener : BaseCaptureComponent
{
    Queue<Vector3> _noiseQueue = new Queue<Vector3>();
    Action OnNoiseReceived;
    int _maxQueueSize;

    public override void Initialize(float radius, int maxQueueSize, Action OnNoiseReceived)
    {
        _captureRadius = radius;
        _maxQueueSize = maxQueueSize;
        this.OnNoiseReceived = OnNoiseReceived;
    }

    protected override bool IsAlreadyContaining(Collider other)
    {
        if (IsQueueEmpty() == true) return false;
        return _noiseQueue.Contains(other.transform.position);
    }

    public void ClearAllNoise()
    {
        _noiseQueue.Clear();
    }
    public bool IsQueueEmpty()
    {
        return _noiseQueue.Count == 0;
    }

    public Vector3 ReturnFrontNoise()
    {
        return _noiseQueue.Dequeue();
    }

    bool NowOverMaxQueueSize()
    {
        return _noiseQueue.Count > _maxQueueSize;
    }

    protected override void OnTargetEnter(Collider other)
    {
        if(NowOverMaxQueueSize()) _noiseQueue.Dequeue(); // 최대 크기를 넘기면 Queue에서 빼줌

        _noiseQueue.Enqueue(other.transform.position);
        OnNoiseReceived?.Invoke();
    }
}

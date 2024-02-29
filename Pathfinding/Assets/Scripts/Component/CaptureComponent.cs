using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CaptureComponent<T> : MonoBehaviour
{
    [SerializeField] List<T> _components;
    CapsuleCollider _captureCollider;

    public void Initialize(float captureRadius)
    {
        _components = new List<T>();
        _captureCollider = GetComponent<CapsuleCollider>();
        _captureCollider.radius = captureRadius;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform == transform.parent) return;
        T component = other.GetComponent<T>();
        if (component == null) return;

        _components.Add(component);
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.transform == transform.parent) return;
        T component = other.GetComponent<T>();
        if (component == null) return;

        _components.Remove(component);
    }

    public List<T> ReturnNearComponents() { return _components; }
}

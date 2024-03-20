using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node : IItem<Node>
{
    public Node(Vector3 pos, Vector3 surfacePos, bool canPass)
    {
        _pos = pos;
        _surfacePos = surfacePos;
        _canPass = canPass;
        StoredIndex = -1;
    }

    Vector3 _pos; // �׸��� ���� ��ġ
    public Vector3 Pos { get { return _pos; } }

    Vector3 _surfacePos; // ���� ���� �� �ִ� ǥ�� ��ġ
    public Vector3 SurfacePos { set { _surfacePos = value; } get { return _surfacePos; } }

    bool _canPass;
    public bool CanPass { get { return _canPass; } }

    // g�� ���� �������� �Ÿ�
    // h�� �� �������� �Ÿ�
    // f�� �� ���� ��ģ ��
    float g, h = 0;
    public float G { get { return g; } set { g = value; } }
    public float H { get { return h; } set { h = value; } }
    public float F { get { return g + h; } }

    public Node ParentNode { get; set; }
    public int StoredIndex { get; set; }

    public int CompareTo(Node other)
    {
        int compareValue = F.CompareTo(other.F);
        if (compareValue == 0) compareValue = H.CompareTo(other.H);
        return compareValue;
    }

    public void Dispose()
    {
        StoredIndex = -1;
        ParentNode = null;
    }
}
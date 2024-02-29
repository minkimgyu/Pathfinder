using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid
{
    Vector3 _groundPos; // �׸����� ���� �� ��ġ
    public Vector3 GroundPos { get { return _groundPos; } }

    public Vector3 WorldPos { get { return _groundPos + new Vector3(0, _topOffset, 0); } } // �����ɽ����� ���� ���� ��ġ

    bool _isOverlap; // �ٸ� ������Ʈ�� �����ִ� ���
    public bool IsOverlap { get { return _isOverlap; } } // �ٸ� ������Ʈ�� �����ִ� ���

    bool _canPass; // �ٸ� ������Ʈ�� ���������� ����� ������ ���
    public bool CanPass { get { return _canPass; } } // �ٸ� ������Ʈ�� ���������� ����� ������ ���

    float _topOffset; // �׸����� ���� �� z��

    // g�� ���� �������� �Ÿ�
    // h�� �� �������� �Ÿ�
    // f�� �� ���� ��ģ ��
    float g, h = 0;
    public float G { get { return g; } set { g = value; } }
    public float H { get { return h; } set { h = value; } }
    public float F { get { return g + h; } }

    public Grid ParentGrid { get; set; }

    public Grid(Vector3 position, float topOffset, bool isOverlap, bool canPass)
    {
        _groundPos = position;
        _topOffset = topOffset;
        _isOverlap = isOverlap;
        _canPass = canPass;
    }
}

public class GridSystem : MonoBehaviour
{
    Grid[,] _grids;
    [SerializeField] Transform _bottomLeftTarget;
    float BottomPos { get { return _bottomLeftTarget.position.y; } }

    [SerializeField] float _gridCellSize; // Grid cell �ϳ��� ũ�� (width, height ����)

    [Range(0f, 90f)]
    [SerializeField] float _maxAngleBetweenGrid; // ��밡���� Grid cell ���� �ִ� ���� ����

    [SerializeField] int _maxTopGridCount; // Grid�� ����ϴ� Y�� �׸��� ����

    [SerializeField] Vector2Int _gridSize; // Grid�� width, height ����
    Vector2Int GridSize { get { return _gridSize; } }

    [SerializeField] Color _passGridColor; // Grid�� ����
    [SerializeField] Color _cantPassGridColor; // ��ġ�� Grid�� ����

    [SerializeField] LayerMask _overlapLayerMask; // ��ġ�� ������Ʈ�� ���̾�

    // Start is called before the first frame update
    void Awake() => CreateGrid();

    public bool IsInMaxAngleBetweenGrid(Vector3 targetGridPos, Vector3 nearGridPos)
    {
        float yDistance = nearGridPos.y - targetGridPos.y;
        if(yDistance > 0) // �ֺ��� �� ���� ���
        {
            float angle = Mathf.Atan2(yDistance, _gridCellSize) * Mathf.Rad2Deg; // �ݹ��ؼ� _gridCellSize�� width��
            return angle <= _maxAngleBetweenGrid;
        }
        else return true; // Ÿ�� �׸��尡 �� ���� ���
    }

    public Grid ReturnGrid(Vector2Int index) { return _grids[index.x, index.y]; }

    void GetTwoCornerWorldPos(out Vector3 bottomLeftPos, out Vector3 topRightPos)
    {
        bottomLeftPos = _grids[0, 0].GroundPos;
        topRightPos = _grids[GridSize.x - 1, GridSize.y - 1].GroundPos;
    }

    public Vector3 ReturnCloestGridPos(Vector3 pos)
    {
        Vector3 bottomLeftPos, topRightPos;
        GetTwoCornerWorldPos(out bottomLeftPos, out topRightPos);

        // �ݿø��ϰ� ���� �ȿ� ������
        // �� �κ��� GridSize �ٲ�� �����ؾ���
        float xPos = Mathf.Clamp(Mathf.Round(pos.x), bottomLeftPos.x, topRightPos.x);
        float zPos = Mathf.Clamp(Mathf.Round(pos.z), bottomLeftPos.z, topRightPos.z);

        return new Vector3(xPos, pos.y, zPos);
    }

    // �ݿø��� �� ��, ���� ������ ����� �ȴٸ� �ּ�, �ִ� ������ �����ش�.
    public Vector2Int ReturnCloestGridIndex(Vector3 pos)
    {
        Vector3 cloestGridPos = ReturnCloestGridPos(pos);
        return ReturnGridIndex(cloestGridPos);
    }

    public List<Vector2Int> ReturnNearGridIndexes(int xIndex, int yIndex)
    {
        List<Vector2Int> closeGridIndex = new List<Vector2Int>();

        // �ֺ� �׸���
        Vector2Int[] closeIndex = new Vector2Int[] {
            new Vector2Int(xIndex - 1, yIndex + 1), new Vector2Int(xIndex, yIndex + 1), new Vector2Int(xIndex + 1, yIndex + 1),
            new Vector2Int(xIndex - 1, yIndex), new Vector2Int(xIndex + 1, yIndex),
            new Vector2Int(xIndex - 1, yIndex - 1), new Vector2Int(xIndex, yIndex - 1), new Vector2Int(xIndex + 1, yIndex - 1)
        };

        for (int i = 0; i < closeIndex.Length; i++)
        {
            bool isOutOfRange = closeIndex[i].x < 0 || closeIndex[i].y < 0 || closeIndex[i].x > GridSize.x - 1 || closeIndex[i].y > GridSize.y - 1;
            if (isOutOfRange == true) continue;

            closeGridIndex.Add(closeIndex[i]);
        }

        return closeGridIndex;
    }





    public Vector2Int ReturnGridIndex(Vector3 worldPos)
    {
        int xIndex = (int)((worldPos.x - _bottomLeftTarget.position.x) / _gridCellSize);
        int zIndex = (int)((worldPos.z - _bottomLeftTarget.position.z) / _gridCellSize);

        return new Vector2Int(xIndex, zIndex);
    }


    Vector3 ReturnGridWorldPosition(int xIndex, int yIndex)
    {
        float wordX = _bottomLeftTarget.position.x + (xIndex * _gridCellSize);
        float wordZ = _bottomLeftTarget.position.z + (yIndex * _gridCellSize);

        return new Vector3(wordX, BottomPos, wordZ);
    }

    float ReturnGridTopOffset(Vector3 gridWorldPos, out bool isOverlap, out bool canPass)
    {
        Vector3 boxSize = new Vector3(_gridCellSize, _gridCellSize, _gridCellSize);
        Vector3 tmpGridPosition = new Vector3(gridWorldPos.x, _maxTopGridCount, gridWorldPos.z);

        RaycastHit hit;
        Physics.BoxCast(tmpGridPosition, boxSize / 2f, Vector3.down, out hit, Quaternion.identity, 30, _overlapLayerMask);
        //Physics.Raycast(tmpGridPosition, Vector3.down, out hit, 30, _overlapLayerMask);

        if (hit.collider == null)
        {
            isOverlap = false; // �ƹ� �͵� ���� ���� ���
            canPass = true; // ��� ������ ������
            return BottomPos;
        }

        //Debug.DrawRay(tmpGridPosition, Vector3.down * hit.distance, Color.black, 300); // Hit�� �������� ray�� �׷��ش�.

        isOverlap = true; // �Ʒ� ���� ��� ���� �����

        // ������Ʈ�� �ִ���� �����İ� �� �ִ� ��찡 ������
        if(hit.collider.CompareTag("CanPass")) canPass = true;
        else canPass = false;

        return hit.point.y;
    }

    void CreateGrid()
    {
        _grids = new Grid[GridSize.x, GridSize.y];

        for (int i = 0; i < GridSize.x; i++)
        {
            for (int j = 0; j < GridSize.y; j++)
            {
                Vector3 gridWorldPos = ReturnGridWorldPosition(i, j);
                float topOffset = ReturnGridTopOffset(gridWorldPos, out bool nowOverlap, out bool canPass);

                _grids[i, j] = new Grid(gridWorldPos, topOffset, nowOverlap, canPass);
            }
        }
    }

    void DrawLineBetweenGrid(int xIndex, int yIndex)
    {
        Grid nowGrid = _grids[xIndex, yIndex];
        if (nowGrid.CanPass == false) return;

        List<Vector2Int> closeGridIndexes = ReturnNearGridIndexes(xIndex, yIndex);

        for (int i = 0; i < closeGridIndexes.Count; i++)
        {
            if (_grids[closeGridIndexes[i].x, closeGridIndexes[i].y].CanPass == false) continue;

            Vector3 gridPos = _grids[xIndex, yIndex].WorldPos;
            Vector3 closePos = _grids[closeGridIndexes[i].x, closeGridIndexes[i].y].WorldPos;

            bool isIn = IsInMaxAngleBetweenGrid(gridPos, closePos);
            if(isIn == false) continue;

            Gizmos.color = Color.cyan;
            Gizmos.DrawLine(gridPos, closePos);
        }
    }

    

    private void OnDrawGizmos()
    {
        if (Application.isPlaying == false || _grids == null) return;

        // x, y �������� ���� Grid�� �׷���
        for (int i = 0; i < GridSize.x; i++)
        {
            for (int j = 0; j < GridSize.y; j++)
            {
                if (_grids[i, j].IsOverlap) // ��ġ�� ���
                {
                    if(_grids[i, j].CanPass == true) // ������ �� ����
                    {
                        DrawLineBetweenGrid(i, j);
                        Gizmos.color = _passGridColor;
                    }
                    else
                    {
                        Gizmos.color = _cantPassGridColor; // �� ������
                    }
                }
                else Gizmos.color = _passGridColor;

                Gizmos.DrawCube(_grids[i, j].WorldPos, new Vector3(_gridCellSize, 0.1f, _gridCellSize));
            }
        }   
    }
}

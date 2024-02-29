using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid
{
    Vector3 _groundPos; // 그리드의 월드 상 위치
    public Vector3 GroundPos { get { return _groundPos; } }

    public Vector3 WorldPos { get { return _groundPos + new Vector3(0, _topOffset, 0); } } // 레이케스팅을 통해 구한 위치

    bool _isOverlap; // 다른 오브젝트와 겹쳐있는 경우
    public bool IsOverlap { get { return _isOverlap; } } // 다른 오브젝트와 겹쳐있는 경우

    bool _canPass; // 다른 오브젝트와 겹쳐있지만 통과가 가능한 경우
    public bool CanPass { get { return _canPass; } } // 다른 오브젝트와 겹쳐있지만 통과가 가능한 경우

    float _topOffset; // 그리드의 월드 상 z값

    // g는 시작 노드부터의 거리
    // h는 끝 노드부터의 거리
    // f는 이 둘을 합친 값
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

    [SerializeField] float _gridCellSize; // Grid cell 하나의 크기 (width, height 통합)

    [Range(0f, 90f)]
    [SerializeField] float _maxAngleBetweenGrid; // 허용가능한 Grid cell 간의 최대 각도 차이

    [SerializeField] int _maxTopGridCount; // Grid가 허용하는 Y축 그리드 개수

    [SerializeField] Vector2Int _gridSize; // Grid의 width, height 개수
    Vector2Int GridSize { get { return _gridSize; } }

    [SerializeField] Color _passGridColor; // Grid의 색상
    [SerializeField] Color _cantPassGridColor; // 겹치는 Grid의 색상

    [SerializeField] LayerMask _overlapLayerMask; // 겹치는 오브젝트의 레이어

    // Start is called before the first frame update
    void Awake() => CreateGrid();

    public bool IsInMaxAngleBetweenGrid(Vector3 targetGridPos, Vector3 nearGridPos)
    {
        float yDistance = nearGridPos.y - targetGridPos.y;
        if(yDistance > 0) // 주변이 더 높은 경우
        {
            float angle = Mathf.Atan2(yDistance, _gridCellSize) * Mathf.Rad2Deg; // 반반해서 _gridCellSize가 width임
            return angle <= _maxAngleBetweenGrid;
        }
        else return true; // 타겟 그리드가 더 높은 경우
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

        // 반올림하고 범위 안에 맞춰줌
        // 이 부분은 GridSize 바뀌면 수정해야함
        float xPos = Mathf.Clamp(Mathf.Round(pos.x), bottomLeftPos.x, topRightPos.x);
        float zPos = Mathf.Clamp(Mathf.Round(pos.z), bottomLeftPos.z, topRightPos.z);

        return new Vector3(xPos, pos.y, zPos);
    }

    // 반올림을 할 때, 값이 범위를 벗어나게 된다면 최소, 최대 값으로 맞춰준다.
    public Vector2Int ReturnCloestGridIndex(Vector3 pos)
    {
        Vector3 cloestGridPos = ReturnCloestGridPos(pos);
        return ReturnGridIndex(cloestGridPos);
    }

    public List<Vector2Int> ReturnNearGridIndexes(int xIndex, int yIndex)
    {
        List<Vector2Int> closeGridIndex = new List<Vector2Int>();

        // 주변 그리드
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
            isOverlap = false; // 아무 것도 닿지 않은 경우
            canPass = true; // 통과 가능한 상태임
            return BottomPos;
        }

        //Debug.DrawRay(tmpGridPosition, Vector3.down * hit.distance, Color.black, 300); // Hit된 지점까지 ray를 그려준다.

        isOverlap = true; // 아래 경우는 모두 닿은 경우임

        // 오브젝트가 있더라고 지나쳐갈 수 있는 경우가 존재함
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

        // x, y 기준으로 나온 Grid를 그려줌
        for (int i = 0; i < GridSize.x; i++)
        {
            for (int j = 0; j < GridSize.y; j++)
            {
                if (_grids[i, j].IsOverlap) // 겹치는 경우
                {
                    if(_grids[i, j].CanPass == true) // 지나갈 수 있음
                    {
                        DrawLineBetweenGrid(i, j);
                        Gizmos.color = _passGridColor;
                    }
                    else
                    {
                        Gizmos.color = _cantPassGridColor; // 못 지나감
                    }
                }
                else Gizmos.color = _passGridColor;

                Gizmos.DrawCube(_grids[i, j].WorldPos, new Vector3(_gridCellSize, 0.1f, _gridCellSize));
            }
        }   
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Pathfinder : MonoBehaviour
{
    //Func<Vector3, Vector3> ReturnCloestGridPos;
    Func<Vector3, Vector2Int> ReturnCloestGridIndex;
    Func<int, int, List<Vector2Int>> ReturnNearGridIndexes;
    Func<Vector3, Vector2Int> ReturnGridIndex;

    Func<Vector3, Vector3, bool> IsInMaxAngleBetweenGrid;

    Func<Vector2Int, Grid> ReturnGrid;

    List<Grid> _openList = new List<Grid>();
    List<Grid> _closedList = new List<Grid>();
    List<Grid> _finalList = new List<Grid>();

    // Start is called before the first frame update
    void Awake()
    {
        GridSystem gridSystem = GetComponent<GridSystem>();
        //ReturnCloestGridPos = gridSystem.ReturnCloestGridPos;
        ReturnCloestGridIndex = gridSystem.ReturnCloestGridIndex;
        ReturnNearGridIndexes = gridSystem.ReturnNearGridIndexes;
        ReturnGridIndex = gridSystem.ReturnGridIndex;
        IsInMaxAngleBetweenGrid = gridSystem.IsInMaxAngleBetweenGrid;

        ReturnGrid = gridSystem.ReturnGrid;
    }

    List<Vector3> ConvertGridToV3(List<Grid> grids)
    {
        List<Vector3> points = new List<Vector3>();
        for (int i = 0; i < grids.Count; i++)
            points.Add(grids[i].WorldPos);

        return points;
    }

    // 가장 먼저 반올림을 통해 가장 가까운 노드를 찾는다.
    public List<Vector3> FindPath(Vector3 startPos, Vector3 targetPos)
    {
        // 리스트 초기화
        _openList.Clear();
        _closedList.Clear();
        _finalList.Clear();

        Vector2Int startIndex = ReturnCloestGridIndex(startPos);
        Vector2Int endIndex = ReturnCloestGridIndex(targetPos);

        Grid startGrid = ReturnGrid(startIndex);
        Grid endGrid = ReturnGrid(endIndex);
        _openList.Add(startGrid);

        while (_openList.Count > 0)
        {
            if (Input.GetKeyDown(KeyCode.Escape)) break;

            Grid targetGrid = ReturnMinFGridInList();
            if (targetGrid == endGrid) // 목적지와 타겟이 같으면 끝
            {
                Grid TargetCurNode = targetGrid;
                while (TargetCurNode != startGrid)
                {
                    _finalList.Add(TargetCurNode);
                    TargetCurNode = TargetCurNode.ParentGrid;
                }
                _finalList.Add(startGrid);
                _finalList.Reverse();

                return ConvertGridToV3(_finalList);
            }

            _openList.Remove(targetGrid); // 해당 그리드 지워줌
            _closedList.Add(targetGrid); // 해당 그리드 추가해줌
            AddNearGridInList(targetGrid, endGrid.GroundPos); // 주변 그리드를 찾아서 다시 넣어줌
        }


        // 이 경우는 경로를 찾지 못한 상황임
        return null;
    }

    void AddNearGridInList(Grid targetGrid, Vector3 targetGridPos)
    {
        Vector2Int index = ReturnGridIndex(targetGrid.GroundPos);
        List<Vector2Int> nearGridIndexes = ReturnNearGridIndexes(index.x, index.y);

        for (int i = 0; i < nearGridIndexes.Count; i++)
        {
            Grid nearGrid = ReturnGrid(nearGridIndexes[i]);
            if (nearGrid.CanPass == false || _closedList.Contains(nearGrid)) continue; // 통과하지 못하거나 닫힌 리스트에 있는 경우 다음 그리드 탐색

            bool isIn = IsInMaxAngleBetweenGrid(targetGrid.WorldPos, nearGrid.WorldPos);
            if (isIn == false) continue;

            // 이 부분 중요! --> 거리를 측정해서 업데이트 하지 않고 계속 더해주는 방식으로 진행해야함
            float moveCost = Vector3.Distance(targetGrid.WorldPos, nearGrid.WorldPos);
            moveCost += targetGrid.G;

            bool isOpenListContainNearGrid = _openList.Contains(nearGrid);

            // 오픈 리스트에 있더라도 G 값이 변경된다면 다시 리셋해주기
            if (moveCost < nearGrid.G || isOpenListContainNearGrid == false)
            {
                // 여기서 grid 값 할당 필요
                nearGrid.G = moveCost;
                nearGrid.H = Vector3.Distance(nearGrid.WorldPos, targetGridPos);
                nearGrid.ParentGrid = targetGrid;

                // 다시 값을 넣는 것 방지
                if(isOpenListContainNearGrid == false) _openList.Add(nearGrid);
            }
        }
    }

    Grid ReturnMinFGridInList()
    {
        if (_openList.Count == 1) return _openList[0];

        Grid resultGrid = _openList[0];
        for (int i = 0; i < _openList.Count; i++)
        {
            if (_openList[i].F <= resultGrid.F && _openList[i].H < resultGrid.H) resultGrid = _openList[i];
        }

        return resultGrid;
    }


    private void OnDrawGizmos()
    {
        if (Application.isPlaying == false) return;

        for (int i = 0; i < _finalList.Count; i++)
        {
            Gizmos.DrawCube(_finalList[i].WorldPos, new Vector3(1, 1, 1));
        }
    }
}

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

    // ���� ���� �ݿø��� ���� ���� ����� ��带 ã�´�.
    public List<Vector3> FindPath(Vector3 startPos, Vector3 targetPos)
    {
        // ����Ʈ �ʱ�ȭ
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
            if (targetGrid == endGrid) // �������� Ÿ���� ������ ��
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

            _openList.Remove(targetGrid); // �ش� �׸��� ������
            _closedList.Add(targetGrid); // �ش� �׸��� �߰�����
            AddNearGridInList(targetGrid, endGrid.GroundPos); // �ֺ� �׸��带 ã�Ƽ� �ٽ� �־���
        }


        // �� ���� ��θ� ã�� ���� ��Ȳ��
        return null;
    }

    void AddNearGridInList(Grid targetGrid, Vector3 targetGridPos)
    {
        Vector2Int index = ReturnGridIndex(targetGrid.GroundPos);
        List<Vector2Int> nearGridIndexes = ReturnNearGridIndexes(index.x, index.y);

        for (int i = 0; i < nearGridIndexes.Count; i++)
        {
            Grid nearGrid = ReturnGrid(nearGridIndexes[i]);
            if (nearGrid.CanPass == false || _closedList.Contains(nearGrid)) continue; // ������� ���ϰų� ���� ����Ʈ�� �ִ� ��� ���� �׸��� Ž��

            bool isIn = IsInMaxAngleBetweenGrid(targetGrid.WorldPos, nearGrid.WorldPos);
            if (isIn == false) continue;

            // �� �κ� �߿�! --> �Ÿ��� �����ؼ� ������Ʈ ���� �ʰ� ��� �����ִ� ������� �����ؾ���
            float moveCost = Vector3.Distance(targetGrid.WorldPos, nearGrid.WorldPos);
            moveCost += targetGrid.G;

            bool isOpenListContainNearGrid = _openList.Contains(nearGrid);

            // ���� ����Ʈ�� �ִ��� G ���� ����ȴٸ� �ٽ� �������ֱ�
            if (moveCost < nearGrid.G || isOpenListContainNearGrid == false)
            {
                // ���⼭ grid �� �Ҵ� �ʿ�
                nearGrid.G = moveCost;
                nearGrid.H = Vector3.Distance(nearGrid.WorldPos, targetGridPos);
                nearGrid.ParentGrid = targetGrid;

                // �ٽ� ���� �ִ� �� ����
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

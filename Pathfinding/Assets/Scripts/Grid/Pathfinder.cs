using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Grid
{
    public class Pathfinder : MonoBehaviour
    {
        Func<Vector3Int, List<Vector3Int>> ReturnNearNodeIndexes;
        Func<Vector3, Vector3Int> ReturnNodeIndex;

        Func<Vector3Int, Node> ReturnNode;

        List<Node> _openList = new List<Node>();
        List<Node> _closedList = new List<Node>();
        List<Node> _finalList = new List<Node>();

        [SerializeField] int openListCount = 0;

        // Start is called before the first frame update
        void Awake()
        {
            GridManager gridManager = GetComponent<GridManager>();
            ReturnNearNodeIndexes = gridManager.ReturnNearNodeIndexes;
            ReturnNodeIndex = gridManager.ReturnNodeIndex;

            ReturnNode = gridManager.ReturnNode;
        }

        List<Vector3> ConvertNodeToV3(List<Node> grids)
        {
            List<Vector3> points = new List<Vector3>();
            for (int i = 0; i < grids.Count; i++)
                points.Add(grids[i].SurfacePos);

            return points;
        }

        // 가장 먼저 반올림을 통해 가장 가까운 노드를 찾는다.
        public List<Vector3> FindPath(Vector3 startPos, Vector3 targetPos)
        {
            // 리스트 초기화
            _openList.Clear();
            _closedList.Clear();
            _finalList.Clear();

            Vector3Int startIndex = ReturnNodeIndex(startPos);
            Vector3Int endIndex = ReturnNodeIndex(targetPos);

            Node startGrid = ReturnNode(startIndex);
            Node endGrid = ReturnNode(endIndex);
            _openList.Add(startGrid);

            while (_openList.Count > 0)
            {
                Node targetGrid = ReturnMinFGridInList();
                if (targetGrid == endGrid) // 목적지와 타겟이 같으면 끝
                {
                    Node TargetCurNode = targetGrid;
                    while (TargetCurNode != startGrid)
                    {
                        _finalList.Add(TargetCurNode);
                        TargetCurNode = TargetCurNode.ParentNode;
                    }
                    _finalList.Add(startGrid);
                    _finalList.Reverse();

                    return ConvertNodeToV3(_finalList);
                }

                _openList.Remove(targetGrid); // 해당 그리드 지워줌
                _closedList.Add(targetGrid); // 해당 그리드 추가해줌
                AddNearGridInList(targetGrid, endGrid.SurfacePos); // 주변 그리드를 찾아서 다시 넣어줌
            }


            // 이 경우는 경로를 찾지 못한 상황임
            return null;
        }

        void AddNearGridInList(Node targetGrid, Vector3 targetGridPos)
        {
            Vector3Int index = ReturnNodeIndex(targetGrid.SurfacePos);
            List<Vector3Int> nearGridIndexes = ReturnNearNodeIndexes(index);

            for (int i = 0; i < nearGridIndexes.Count; i++)
            {
                Node nearGrid = ReturnNode(nearGridIndexes[i]);
                if (nearGrid.CanPass == false || _closedList.Contains(nearGrid)) continue; // 통과하지 못하거나 닫힌 리스트에 있는 경우 다음 그리드 탐색

                // 이 부분 중요! --> 거리를 측정해서 업데이트 하지 않고 계속 더해주는 방식으로 진행해야함
                float moveCost = Vector3.Distance(targetGrid.SurfacePos, nearGrid.SurfacePos);
                moveCost += targetGrid.G;

                bool isOpenListContainNearGrid = _openList.Contains(nearGrid);

                // 오픈 리스트에 있더라도 G 값이 변경된다면 다시 리셋해주기
                if (moveCost < nearGrid.G || isOpenListContainNearGrid == false)
                {
                    // 여기서 grid 값 할당 필요
                    nearGrid.G = moveCost;
                    nearGrid.H = Vector3.Distance(nearGrid.SurfacePos, targetGridPos);
                    nearGrid.ParentNode = targetGrid;

                    // 다시 값을 넣는 것 방지
                    if (isOpenListContainNearGrid == false)
                    {
                        _openList.Add(nearGrid);
                        if (openListCount < _openList.Count) openListCount = _openList.Count;
                    }
                }
            }
        }

        Node ReturnMinFGridInList()
        {
            if (_openList.Count == 1) return _openList[0];

            Node resultGrid = _openList[0];
            for (int i = 0; i < _openList.Count; i++)
            {
                if (_openList[i].F <= resultGrid.F && _openList[i].H < resultGrid.H) resultGrid = _openList[i];
            }

            return resultGrid;
        }
    }
}
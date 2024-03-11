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

        // ���� ���� �ݿø��� ���� ���� ����� ��带 ã�´�.
        public List<Vector3> FindPath(Vector3 startPos, Vector3 targetPos)
        {
            // ����Ʈ �ʱ�ȭ
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
                if (targetGrid == endGrid) // �������� Ÿ���� ������ ��
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

                _openList.Remove(targetGrid); // �ش� �׸��� ������
                _closedList.Add(targetGrid); // �ش� �׸��� �߰�����
                AddNearGridInList(targetGrid, endGrid.SurfacePos); // �ֺ� �׸��带 ã�Ƽ� �ٽ� �־���
            }


            // �� ���� ��θ� ã�� ���� ��Ȳ��
            return null;
        }

        void AddNearGridInList(Node targetGrid, Vector3 targetGridPos)
        {
            Vector3Int index = ReturnNodeIndex(targetGrid.SurfacePos);
            List<Vector3Int> nearGridIndexes = ReturnNearNodeIndexes(index);

            for (int i = 0; i < nearGridIndexes.Count; i++)
            {
                Node nearGrid = ReturnNode(nearGridIndexes[i]);
                if (nearGrid.CanPass == false || _closedList.Contains(nearGrid)) continue; // ������� ���ϰų� ���� ����Ʈ�� �ִ� ��� ���� �׸��� Ž��

                // �� �κ� �߿�! --> �Ÿ��� �����ؼ� ������Ʈ ���� �ʰ� ��� �����ִ� ������� �����ؾ���
                float moveCost = Vector3.Distance(targetGrid.SurfacePos, nearGrid.SurfacePos);
                moveCost += targetGrid.G;

                bool isOpenListContainNearGrid = _openList.Contains(nearGrid);

                // ���� ����Ʈ�� �ִ��� G ���� ����ȴٸ� �ٽ� �������ֱ�
                if (moveCost < nearGrid.G || isOpenListContainNearGrid == false)
                {
                    // ���⼭ grid �� �Ҵ� �ʿ�
                    nearGrid.G = moveCost;
                    nearGrid.H = Vector3.Distance(nearGrid.SurfacePos, targetGridPos);
                    nearGrid.ParentNode = targetGrid;

                    // �ٽ� ���� �ִ� �� ����
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
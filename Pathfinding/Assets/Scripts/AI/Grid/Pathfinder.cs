using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


namespace Grid.Pathfinder
{

    public class Pathfinder : MonoBehaviour
    {
        Func<Vector3Int, List<Vector3Int>> ReturnNearNodeIndexes;
        Func<Vector3, Vector3Int> ReturnNodeIndex;

        Func<Vector3Int, Node> ReturnNode;

        const int maxSize = 1000;

        Heap<Node> _openList = new Heap<Node>(maxSize);
        HashSet<Node> _closedList = new HashSet<Node>();

        // Start is called before the first frame update
        void Awake()
        {
            GridManager gridManager = GetComponent<GridManager>();
            ReturnNearNodeIndexes = gridManager.ReturnNearNodeIndexes;
            ReturnNodeIndex = gridManager.ReturnNodeIndex;

            ReturnNode = gridManager.ReturnNode;
        }

        List<Vector3> ConvertNodeToV3(Stack<Node> stackNode)
        {
            List<Vector3> points = new List<Vector3>();
            while (stackNode.Count > 0)
            {
                Node node = stackNode.Peek();
                points.Add(node.SurfacePos);
                stackNode.Pop();
            }

            return points;
        }

        // ���� ���� �ݿø��� ���� ���� ����� ��带 ã�´�.
        public List<Vector3> FindPath(Vector3 startPos, Vector3 targetPos)
        {
            //// ����Ʈ �ʱ�ȭ
            _openList.Clear();
            _closedList.Clear();

            Vector3Int startIndex = ReturnNodeIndex(startPos);
            Vector3Int endIndex = ReturnNodeIndex(targetPos);

            Node startNode = ReturnNode(startIndex);
            Node endNode = ReturnNode(endIndex);
            _openList.Insert(startNode);

            while (_openList.Count > 0)
            {
                Node targetNode = _openList.ReturnMin();
                if (targetNode == endNode) // �������� Ÿ���� ������ ��
                {
                    Stack<Node> finalList = new Stack<Node>();

                    Node TargetCurNode = targetNode;
                    while (TargetCurNode != startNode)
                    {
                        finalList.Push(TargetCurNode);
                        TargetCurNode = TargetCurNode.ParentNode;
                    }
                    //finalList.Push(startNode);

                    return ConvertNodeToV3(finalList);
                }

                _openList.DeleteMin(); // �ش� �׸��� ������
                _closedList.Add(targetNode); // �ش� �׸��� �߰�����
                AddNearGridInList(targetNode, endNode.SurfacePos); // �ֺ� �׸��带 ã�Ƽ� �ٽ� �־���
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
                Node nearNode = ReturnNode(nearGridIndexes[i]);
                if (nearNode.CanPass == false || _closedList.Contains(nearNode)) continue; // ������� ���ϰų� ���� ����Ʈ�� �ִ� ��� ���� �׸��� Ž��

                // �� �κ� �߿�! --> �Ÿ��� �����ؼ� ������Ʈ ���� �ʰ� ��� �����ִ� ������� �����ؾ���
                float moveCost = Vector3.Distance(targetGrid.SurfacePos, nearNode.SurfacePos);
                moveCost += targetGrid.G;

                bool isOpenListContainNearGrid = _openList.Contain(nearNode);

                // ���� ����Ʈ�� �ִ��� G ���� ����ȴٸ� �ٽ� �������ֱ�
                if (isOpenListContainNearGrid == false || moveCost < nearNode.G)
                {
                    // ���⼭ grid �� �Ҵ� �ʿ�
                    nearNode.G = moveCost;
                    nearNode.H = Vector3.Distance(nearNode.SurfacePos, targetGridPos);
                    nearNode.ParentNode = targetGrid;
                }

                if (isOpenListContainNearGrid == false) _openList.Insert(nearNode);
            }
        }
    }
}
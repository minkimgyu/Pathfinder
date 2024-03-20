using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Grid.Pathfinder;

namespace AI.Flock
{
    public class Flock : MonoBehaviour
    {
        [SerializeField] FlockAgent _agentPrefab;
        [SerializeField] List<Transform> _targets;

        [SerializeField] List<FlockAgent> _agents;

        [Range(0f, 30f)]
        [SerializeField] int _areaSize = 30;

        [Range(10, 500)]
        [SerializeField] int _startingCount = 50;

        [Range(0f, 1f)]
        [SerializeField] float _agentDensity = 0.08f;

        [Range(1f, 10f)]
        [SerializeField] float _agentCaptureRadius = 2f;

        [Range(1f, 100f)]
        [SerializeField] float _agentMoveSpeed = 3f;

        [Range(1f, 10f)]
        [SerializeField] float _agentViewSpeed = 3f;

        [Range(1f, 10f)]
        [SerializeField] float _goToPathFollowerStateDistance = 7f;

        [Range(1f, 10f)]
        [SerializeField] float _goToFlockFollowerStateDistance = 1.5f;

        void Start() => Invoke("Spawn", 1f);

        void Spawn()
        {
            Pathfinder pathfinder = FindObjectOfType<Pathfinder>();

            Vector3 spawnPoint = new Vector3();

            for (int i = 0; i < _startingCount; i++)
            {
                Vector2 posInCircle = Random.insideUnitCircle * _startingCount * _agentDensity;
                spawnPoint.Set(posInCircle.x, 0f, posInCircle.y);

                FlockAgent newAgent = Instantiate(
                    _agentPrefab,
                    spawnPoint,
                    Quaternion.Euler(Vector3.up * Random.Range(0f, 360f)),
                    transform
                    );

                newAgent.name = "Agent " + i;
                newAgent.Initialize(_targets[0], _agentCaptureRadius, _agentMoveSpeed, _agentViewSpeed);
                _agents.Add(newAgent);
            }

            //_agents[0].OnResetLeader();
            //ChangeLeader(0);
        }

        public List<Transform> ReturnTargets() { return _targets; }
        public void ChangeLeader(int index)
        {
            FlockAgent leaderAgent = _agents[index];
            for (int i = 0; i < _agents.Count; i++)
            {
                //_agents[i].ResetLeader(leaderAgent);
            }
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = new Color(1f, 0, 0, 0.1f);
            Gizmos.DrawSphere(Vector3.zero, _startingCount * _agentDensity);

            Gizmos.color = new Color(0, 0, 1f);
            Gizmos.DrawWireSphere(Vector3.zero, _areaSize);
        }
    }
}
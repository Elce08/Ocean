using System.Collections.Generic;
using UnityEngine;

public class BoidsGenerator : MonoBehaviour
{
    [System.Serializable]
    struct SpawnArea
    {
        public float _min, _max;
    }

    [Header("Boids")]
    [SerializeField]
    Transform _boids;
    [SerializeField]
    float _speed = 5f;
    [SerializeField]
    LayerMask _boidsLayer;

    [Header("Range")]
    [SerializeField, Range(0, 100f)]
    float _detectRange = 10f;
    [SerializeField, Range(0, 100f)]
    float _separationRange = 5f;

    [Header("Spawn")]
    [SerializeField, Range(1, 1000)]
    int _spawnCount;
    [SerializeField]
    SpawnArea _spawnAreaPosX;
    [SerializeField]
    SpawnArea _spawnAreaPosY;
    [SerializeField]
    SpawnArea _spawnAreaPosZ;

    List<Transform> _boidAgents = new();

    AlignmentRule _alignmentRule = new();
    CohesionRule _cohesionRule = new();
    SeparationRule _separtionRule = new();

    private void Awake()
    {
        SpawnBoids();
    }

    void SpawnBoids()
    {
        for (int i = 0; i < _spawnCount; i++)
        {
            Vector3 spawnPos = new Vector3(
                Random.Range(_spawnAreaPosX._min, _spawnAreaPosX._max),
                Random.Range(_spawnAreaPosY._min, _spawnAreaPosY._max),
                Random.Range(_spawnAreaPosZ._min, _spawnAreaPosZ._max));

            _boidAgents.Add(Instantiate(_boids, spawnPos, Quaternion.identity));
        }
    }

    private void Update()
    {
        foreach (var agent in _boidAgents)
        {
            Vector3 dir = _cohesionRule.GetDirection(
                    agent,
                    GetNeighbor(agent, _detectRange)
                    );

            dir += _alignmentRule.GetDirection(
                agent,
                GetNeighbor(agent, _detectRange)
                );

            dir += _separtionRule.GetDirection(
                agent,
                GetNeighbor(agent, _separationRange)
                );

            dir = Vector3.Lerp(agent.transform.forward, dir, Time.deltaTime);
            dir.Normalize();

            agent.transform.position += dir * _speed * Time.deltaTime;
            agent.transform.rotation = Quaternion.LookRotation(dir);
        }
    }

    List<Transform> GetNeighbor(Transform agent, float range)
    {
        var overlaps = Physics.OverlapSphere(agent.position, range, _boidsLayer);

        if (overlaps.Length == 0)
            return null;

        List<Transform> tf = new List<Transform>(overlaps.Length);

        for (int i = 0; i < overlaps.Length; i++)
        {
            if (overlaps[i].transform == agent)
                continue;

            tf.Add(overlaps[i].transform);
        }

        return tf;
    }
}

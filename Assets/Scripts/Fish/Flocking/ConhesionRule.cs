using System.Collections.Generic;
using UnityEngine;

public class CohesionRule
{
    public Vector3 GetDirection(Transform agent, List<Transform> neighbor)
    {
        if (agent == null || neighbor == null || neighbor.Count == 0)
            return Vector3.zero;

        Vector3 averagePos = Vector3.zero;

        foreach (var ne in neighbor)
        {
            averagePos += ne.position;
        }

        averagePos /= neighbor.Count;

        return (averagePos - agent.transform.position).normalized;
    }
}
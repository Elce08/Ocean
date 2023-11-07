using System.Collections.Generic;
using UnityEngine;

public class AlignmentRule
{
    public Vector3 GetDirection(Transform agent, List<Transform> neighbor)
    {
        if (agent == null)
            return Vector3.zero;

        // 이웃이 없다면, 전방으로 진행
        if (neighbor == null || neighbor.Count == 0)
            return agent.forward;

        Vector3 neighborDir = Vector3.zero;

        foreach (var ne in neighbor)
        {
            neighborDir += ne.transform.forward;
        }

        return (neighborDir /= neighbor.Count).normalized;
    }
}
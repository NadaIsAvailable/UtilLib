using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChildTracker : MonoBehaviour
{
    // Private fields & properties for info we provide based on all children so they can be calculated once
    // per frame vs once per call.
    Vector3 averagePosition;
    Vector3 averageVelocity;
    IEnumerable<Vector3> childPositions;
    public Vector3 AveragePosition { get { return averagePosition; } }
    public Vector3 AverageVelocity { get { return averageVelocity; } }
    public IEnumerable<Vector3> ChildPositions { get { return childPositions; } }
    public int Count { get { return transform.childCount; } }

    /// <summary>
    /// Each frame, update our calcs about average position and velocity of all children
    /// so that they aren't calculated every time they are requested
    /// </summary>
    private void FixedUpdate()
    {
        // Average position
        averagePosition = Vector3.zero;
        foreach (Transform child in transform)
        {
            averagePosition += child.position;
        }
        averagePosition /= transform.childCount;

        // Average velocity
        averageVelocity = Vector3.zero;
        int agentCount = 0;
        foreach (Agent agent in GetComponentsInChildren<Agent>())
        {
            averageVelocity += agent.velocity;
            agentCount++;
        }
        averageVelocity /= agentCount;

        // Child positions
        List<Vector3> allPositions = new List<Vector3>();
        foreach (Transform child in transform)
        {
            allPositions.Add(child.position);
        }
        childPositions = allPositions;
    }

    /// <summary>
    /// Find the closest child to a given point
    /// </summary>
    public GameObject FindClosestChild(Vector3 position)
    {
        float minDistance = float.MaxValue;
        GameObject closestChild = null;
        foreach (Transform child in transform)
        {
            float distance = Vector3.Distance(child.position, position);
            if (distance < minDistance)
            {
                minDistance = distance;
                closestChild = child.gameObject;
            }
        }
        return closestChild;
    }

    /// <summary>
    /// Find the furthest child from a given point 
    /// </summary>
    public GameObject FindFurthestChild(Vector3 position)
    {
        float maxDistance = float.MinValue;
        GameObject furthestChild = null;
        foreach (Transform child in transform)
        {
            float distance = Vector3.Distance(child.position, position);
            if (distance > maxDistance)
            {
                maxDistance = distance;
                furthestChild = child.gameObject;
            }
        }
        return furthestChild;
    }

    /// <summary>
    /// Get an enumerator of all child positions within a certain distance of a point
    /// </summary>
    public IEnumerable<Vector3> GetChildPositionsNear(Vector3 position, float distance = float.MaxValue)
    {
        foreach (Transform child in transform)
        {
            if (Vector3.Distance(child.position, position) < distance)
            {
                yield return child.position;
            }
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(AveragePosition, 0.1f);
        Gizmos.DrawRay(AveragePosition, AverageVelocity);
    }
}

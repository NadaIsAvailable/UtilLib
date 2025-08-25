using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Wander : Agent
{
    [SerializeField] float wanderDistance;
    [SerializeField] float wanderRadius;
    [SerializeField, Range(0, 5f)] float wanderJitter;
    [SerializeField, Range(0, 1)] float wanderStrength;

    // Debug used
    private Vector3 wanderForce;

    // Update is called once per frame
    void Update()
    {

    }

    protected override Vector3 CalcSteering()
    {
        wanderForce = Wander(wanderDistance, wanderRadius, wanderJitter);
        return wanderForce * wanderStrength;
    }

    private void OnDrawGizmos()
    {
        // Bounds
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(bounds.center, bounds.size);

        // Circle in front of agent
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position + transform.forward * wanderDistance, wanderRadius);

        // Wander target
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(transform.position, transform.position + transform.forward * wanderDistance + wanderTarget);

        // Wander jitter circle
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position + transform.forward * wanderDistance + wanderTarget, wanderJitter);
    }
}
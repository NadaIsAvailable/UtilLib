using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Flock : Agent
{
    // Flee fields
    [SerializeField, Range(0, 1)] float fleeStrength;
    [SerializeField] float fleeDistance;

    public GameObject target;

    // Wander fields
    [SerializeField] float wanderDistance;
    [SerializeField] float wanderRadius;
    [SerializeField, Range(0, 5f)] float wanderJitter;
    [SerializeField, Range(0, 1)] float wanderStrength;

    // Flock fields
    [SerializeField, Range(0, 1)] float separationStrength;
    [SerializeField] float separateDistance;
    [SerializeField, Range(0, 1)] float cohesionStrength;
    [SerializeField, Range(0, 1)] float alignmentStrength;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    protected override Vector3 CalcSteering()
    {
        // Find current flee steering
        Vector3 fleeSteering = Flee(target, fleeDistance) * fleeStrength;

        // Setup wander steering
        Vector3 wanderSteering = Vector3.zero;

        // If the flee steering is zero, meaning it's far away from enemies:
        if (fleeSteering == Vector3.zero)
        {
            //Start wandering
            wanderSteering = Wander(wanderDistance, wanderRadius, wanderJitter) * wanderStrength;
        }

        // Calculate flock steering
        // Find each of the steering separately and multiple by the strength
        Vector3 flockSteering = (Avoid(separateDistance) * separationStrength) +
            (SeekCenter() * cohesionStrength) + (Align() * alignmentStrength);

        // Add up all steering forces
        Vector3 totalSteering = fleeSteering + wanderSteering + flockSteering;

        return totalSteering;
    }

    private void OnDrawGizmos()
    {
        // Flee detection
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, fleeDistance);

        // Separation detection
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, separateDistance);

        // Movement bounds
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(bounds.center, bounds.size);
    }
}

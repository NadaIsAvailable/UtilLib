using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public abstract class Agent : MonoBehaviour, ITrackedChild
{
    [SerializeField] float maxSpeed;
    [SerializeField] Rigidbody rBody;

    public Vector3 velocity, acceleration;

    // Info to stay in bounds
    [SerializeField] bool stayInBounds = false;
    public Bounds bounds;
    [SerializeField, Range(0, 1)] float stayInBoundsStrength = 1f;

    // Wander fields
    protected Vector3 wanderTarget = Vector3.zero;

    // Flock fields
    private ChildTracker siblingTracker;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        // Setup
        Quaternion nextRotation = transform.rotation;
        Vector3 nextPosition = transform.position;

        // Start with acceleration based on the steering force
        acceleration = CalcSteering();

        // Add stay in bounds force if needed
        if (stayInBounds)
        {
            acceleration += StayInBounds() * stayInBoundsStrength;
        }

        // Calc velocity based on accel scaled by time
        velocity += acceleration * Time.fixedDeltaTime;

        // Clamp velocity to min/max speed
        velocity = Vector3.ClampMagnitude(velocity, maxSpeed);

        // Rotate to face the direction of travel
        nextRotation = Quaternion.LookRotation(velocity, Vector3.up);

        //  Use velocity to calc next position
        nextPosition += (velocity * Time.fixedDeltaTime);

        //  Move the Vehicle
        rBody.Move(nextPosition, nextRotation);

        // Zero out acceleration (b/c it's only a field for debugging)
        acceleration = Vector3.zero;
    }

    protected abstract Vector3 CalcSteering();

    protected Vector3 StayInBounds()
    {
        // Info we can use: bounding box, current position
        if (!bounds.Contains(transform.position))
        {
            // if out of bounds
            return Seek(bounds.center);
        }
        return Vector3.zero;
    }

    #region Seek
    protected Vector3 Seek(Vector3 targetPosition)
    {
        // Calculate desired velocity
        // Desired velocity -> a vector pointing from vehicle to its target
        Vector3 desiredVelocity = targetPosition - transform.position;

        // ** Let Agent FixedUpdate do this: Desired velocity must be scaled by maxSpeed
        // This keeps the velocity ALWAYS at max speed, can cause to overshoot
        Vector3 opt1 = desiredVelocity.normalized * maxSpeed;
        // OR
        // This allows the velocity to be below the max speed,
        // meaning it can slow down as it's closer to the target
        //Vector3 opt2 = Vector3.ClampMagnitude(desiredVelocity, maxSpeed);

        desiredVelocity = opt1;

        // Calculate the resultant steering force required to change a
        // current velocity to the desired velocity
        // Return steering force
        return desiredVelocity - velocity;
    }

    protected Vector3 Seek(GameObject target)
    {
        // Call the other version of Seek 
        // which returns the seeking steering force
        // and then return that returned vector. 
        return Seek(target.transform.position);
    }
    #endregion

    #region Arrive
    protected Vector3 Arrive(Vector3 targetPosition, float distance)
    {
        // Calculate desired velocity
        // Desired velocity -> a vector pointing from vehicle to its target
        Vector3 desiredVelocity = targetPosition - transform.position;

        // Check if agent is within the slow down distance
        if (desiredVelocity.sqrMagnitude < distance * distance)
        {
            // Begin to slow down
            // using the ratio of distance between agent and target / slow down distance
            desiredVelocity = desiredVelocity.normalized * (desiredVelocity.magnitude / distance);
        }
        else
        {
            // Continue to steer at maxSpeed
            desiredVelocity = desiredVelocity.normalized * maxSpeed;
        }

        // Calculate the resultant steering force required to change a
        // current velocity to the desired velocity
        // Return steering force
        return desiredVelocity - velocity;
    }

    protected Vector3 Arrive(GameObject target, float distance)
    {
        // Call the other version of Arrive 
        // which returns the seeking steering force
        // and then return that returned vector. 
        return Arrive(target.transform.position, distance);
    }
    #endregion

    #region Flee
    protected Vector3 Flee(Vector3 targetPos, float distance = float.MaxValue)
    {
        // Calculate desired velocity
        Vector3 desiredVelocity = transform.position - targetPos;

        // Don't flee when it's far away from the target
        if (desiredVelocity.sqrMagnitude > distance * distance)
        {
            return Vector3.zero;
        }

        // Continue to steer at maxSpeed
        desiredVelocity = desiredVelocity.normalized * maxSpeed;

        // Return steering force
        return desiredVelocity - velocity;
    }

    protected Vector3 Flee(GameObject target, float distance = float.MaxValue)
    {
        return Flee(target.transform.position, distance);
    }
    #endregion

    #region Wander
    protected Vector3 Wander(float wanderDistance, float wanderRadius, float wanderJitter)
    {
        // Calculate a random displacement
        Vector2 randomPoint = Random.insideUnitCircle * wanderJitter;
        wanderTarget += new Vector3(randomPoint.x, 0, randomPoint.y);

        // Adjust wander target's magnitude to radius of the large circle (wander radius)
        wanderTarget = wanderTarget.normalized * wanderRadius;

        // Calculate the world-space target
        Vector3 desiredPosition = transform.position + transform.forward * wanderDistance + wanderTarget;

        // Seek that target position
        return Seek(desiredPosition);
    }
    #endregion

    #region Flock
    public void SetTracker(ChildTracker tracker)
    {
        siblingTracker = tracker;
    }

    /// <summary>
    /// Avoids other agents when there are other agents in the range
    /// (Separation of the flocking steering behavior)
    /// </summary>
    /// <param name="separateDistance">Avoid other agents within this range</param>
    /// <returns>Steering force away from other agents</returns>
    protected Vector3 Avoid(float separateDistance)
    {
        Vector3 avoid = Vector3.zero;

        // Go through all the agents' position
        foreach (Vector3 siblingPosition in siblingTracker.ChildPositions)
        {
            // Exclude itself
            if ((transform.position - siblingPosition).magnitude > Mathf.Epsilon)
            {
                // Find the fleeing force from the sibiling agent
                Vector3 flee = Flee(siblingPosition, separateDistance);

                // Proportionalize the fleeing force
                flee *= 1 / (transform.position - siblingPosition).magnitude;

                // Add to the total avoidence force
                avoid += flee;
            }
        }

        // Return the final steering force
        return avoid;
    }

    /// <summary>
    /// Seeks the center of the flock
    /// (Cohesion of the flocking steering behavior)
    /// </summary>
    /// <returns>Steering force towards the center of the flock</returns>
    protected Vector3 SeekCenter()
    {
        return Seek(siblingTracker.AveragePosition);
    }

    /// <summary>
    /// Aligns the agent's velocity to the average velocity of the flock
    /// (Alignment of the flocking steering behavior)
    /// </summary>
    /// <returns>Average velocity of the flock</returns>
    protected Vector3 Align()
    {
        return Vector3.ClampMagnitude(siblingTracker.AverageVelocity, maxSpeed); ;
    }
    #endregion

    private void OnDrawGizmos()
    {
        if (velocity.sqrMagnitude >= maxSpeed * maxSpeed)
        {
            Gizmos.color = Color.red;
        }
        else
        {
            Gizmos.color = Color.black;
        }
        Gizmos.DrawRay(transform.position, velocity);
    }
}

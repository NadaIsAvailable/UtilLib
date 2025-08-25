 using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.GraphicsBuffer;


public class Vehicle : MonoBehaviour
{
    // Although vehicle will move with us "taking over", we'll still be 
    //   manipulating the Rigidbody of the vehicle while it's kinematic.
    [SerializeField]
    Rigidbody rBody;

    // Fields for Speed
    [SerializeField]
    float maxSpeed;
    [SerializeField, Tooltip("Speed below which the vehicle will come to a full stop.")]
    float minSpeed;

    // Fields for Acceleration/Deceleration
    [SerializeField, Range(0, 1), Tooltip("The % of the max speed the vehicle can achieve in 1 second.")]
    float accelPercent;
    [SerializeField, Range(0, 1), Tooltip("The % by which the velocity is reduced during 1 second of 0 acceleration.")]
    float decelPercent;

    // Fields for Acceleration & Deceleration can also be in units per second
    // This is how we originally did it in class.
    /*
    [SerializeField] float accelerationRate;
    [SerializeField] float decelerationRate;
    */

    // Fields for Turning
    [SerializeField, Tooltip("The # of degrees/second a moving vehicle can turn.")]
    float turnRate;

    // Fields for particle system
    [SerializeField]
    GameObject smoke;

    [SerializeField]
    bool isSomking = false;

    // ***
    // All remaining fields are private!
    // No other classes should have access to this data about the vehicle.
    // They could potentially break the logic of movement in this script.
    // ***

    // Fields for Input
    private Vector3 movementDirection;

    // Fields for Movement Vectors
    private Vector3 velocity;
    private Vector3 acceleration;

    // Fields for rotation
    private Quaternion rotDelta = Quaternion.identity;
    private float rotDir = 1f;

    // Fields/properties that may help for rotation to terrain
    // Track the vehicle forward too
    [SerializeField] Transform vehicleTransform;

    // for demo, turn raycasting on/off
    [SerializeField] bool isRaycasting = false;
    [SerializeField] float maxTerrainHeight = 101f;
    [SerializeField] LayerMask groundLayerMask;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Move();
    }

    public void Move()
    {
        Vector3 nextPos = transform.position;
        Quaternion nextRot = transform.rotation;

        #region Raycasting
        Vector3 origin = nextPos;
        origin.y = maxTerrainHeight;
        RaycastHit terrainHit;

        // If raycasting AND I hit something
        if (isRaycasting && Physics.Raycast(origin, Vector3.down, out terrainHit, maxTerrainHeight, groundLayerMask))
        {
            nextPos.y = terrainHit.point.y;

            Vector3 normal = terrainHit.normal;

            // Find new forward vector via cross product
            Vector3 newForward = Vector3.Cross(vehicleTransform.right, normal);

            // Calculate the rotation from the new forward and the transform's upward vectors
            Quaternion rotation = Quaternion.LookRotation(newForward, vehicleTransform.up);

            // Update model's rotation
            vehicleTransform.rotation = rotation;
        }
        #endregion

        #region Velocity Calc
        // IF the gas is off (no actions performed)
        // Deccelerate
        if (movementDirection.z == 0f)
        {
            // decel based on units per second
            // velocity *= 1f - (decelerationRate * Time.fixedDeltaTime);

            // decelerate based on % of velocity kept per second
            velocity *= 1 - (decelPercent * Time.fixedDeltaTime);

            // If it gets really small, just zero it out
            if (velocity.sqrMagnitude < minSpeed * minSpeed)
            {
                velocity = Vector3.zero;
            }
        }
        else // Accelerate
        {
            // Movement "formula":
            //transform.forward = movementDirection;

            // accel based on units per second
            //acceleration = accelerationRate * movementDirection.z * transform.forward;

            // accel based on % of max speed
            acceleration = transform.forward * movementDirection.z * accelPercent * maxSpeed;

            // Velocity is speed * direction - not scaled to a per-frame basis.
            // It's on a per-second basis.
            // instant GO
            // velocity = maxSpeed * movementDirection.normalized;
            velocity += acceleration * Time.fixedDeltaTime;

            // limit velocity based on maxSpeed
            velocity = Vector3.ClampMagnitude(velocity, maxSpeed);
        }

        #endregion

        #region Rotation Calc
        // How much to turn by
        float turnAmount = movementDirection.x * turnRate * Time.fixedDeltaTime;

        // Once move backward (flip rotation direction for more realistic xp)
        if (movementDirection.z != 0)
        {
            rotDir = Mathf.Sign(movementDirection.z);
        }

        // Applie rotation direction
        turnAmount *= rotDir;

        // Stop rotation after velocity is zero
        if (velocity.sqrMagnitude == 0f)
        {
            turnAmount = 0f;
        }

        rotDelta = Quaternion.Euler(0f, turnAmount, 0f);

        // Rotate the velocity
        velocity = rotDelta * velocity;
        #endregion

        // Move the vehicle!
        nextPos += velocity * Time.fixedDeltaTime;
        nextRot *= rotDelta;
        nextRot.Normalize();

        rBody.Move(nextPos, nextRot);
    }

    public void OnMove(InputAction.CallbackContext callbackContext)
    {
        Vector2 inputDir = callbackContext.ReadValue<Vector2>();
        movementDirection.z = inputDir.y;
        movementDirection.x = inputDir.x;

        // Active particle system when vehicle's 'gas' is ON
        if (inputDir.y != 0 && isSomking)
        {
            smoke.SetActive(true);
            smoke.GetComponent<ParticleSystem>().Play();
            return;
        }
        if (isSomking)
        {
            smoke.SetActive(false);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, velocity);
    }

    public void RestPosition()
    {
        transform.position = Vector3.zero;
    }
}
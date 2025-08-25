using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Flee : Agent
{
    [SerializeField, Range(0, 1)] float fleeStrength;
    [SerializeField] float fleeDistance;

    public GameObject target;

    protected override Vector3 CalcSteering()
    {
        return Flee(target, fleeDistance) * fleeStrength;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, fleeDistance);

        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(bounds.center, bounds.size);
    }
}

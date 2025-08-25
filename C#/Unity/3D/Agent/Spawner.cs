using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    // Instance prefab
    [SerializeField] GameObject prefab;

    // References needed to assign to each instance
    [SerializeField] GameObject target;
    [SerializeField] ChildTracker childTracker;

    [SerializeField] int numOfInstances;

    // Spawning bounds
    [SerializeField] Bounds bounds;

    // Start is called before the first frame update
    void Start()
    {
        // Create instances when start
        CreateInstances();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /// <summary>
    /// Creates n number of instances
    /// </summary>
    private void CreateInstances()
    {
        for (int i = 0; i < numOfInstances; i++)
        {
            // Instantiate and store reference
            GameObject instance = Instantiate(
                prefab,
                PickRandomPosition(),
                PickRandomRotation(),
                transform);

            // Assign neccesary references
            instance.GetComponent<Flock>().SetTracker(childTracker);
            instance.GetComponent<Flock>().target = target;
            instance.GetComponent<Flock>().bounds = bounds;
        }
    }

    /// <summary>
    /// Picks a random position within the spawning bounds
    /// </summary>
    /// <returns>Random position</returns>
    private Vector3 PickRandomPosition()
    {
        Vector3 pos = new Vector3();
        pos.x = Random.Range(bounds.min.x, bounds.max.x);
        pos.y = Random.Range(bounds.min.y, bounds.max.y);
        pos.z = Random.Range(bounds.min.z, bounds.max.z);
        return pos;
    }

    /// <summary>
    /// Picks a random y rotation
    /// </summary>
    /// <returns>Random y rotation</returns>
    private Quaternion PickRandomRotation()
    {
        Quaternion rot = new Quaternion();
        rot.y = Random.Range(0, 360);
        return rot;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(bounds.center, bounds.size);
    }
}

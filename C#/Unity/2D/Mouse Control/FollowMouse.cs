using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class FollowMouse : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        // Obtain mouse world pos and set z to zero
        Vector3 mouseWorldPos =
            Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        mouseWorldPos.z = 0;

        // Update this gameObj's pos to the pos of the obj following
        gameObject.transform.position = mouseWorldPos;
    }
}

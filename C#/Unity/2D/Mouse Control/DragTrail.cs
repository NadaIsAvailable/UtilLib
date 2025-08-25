using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class DragTrail : MonoBehaviour
{
    /// <summary>
    /// Sprite renderer of the game object
    /// </summary>
    private SpriteRenderer spriteRen;

    /// <summary>
    /// Trail renderer of the game object
    /// </summary>
    private TrailRenderer trailRen;

    /// <summary>
    /// Mouse's dragging state
    /// </summary>
    private bool isDragging = false;

    /// <summary>
    /// Is there is trail renderer attached to the object
    /// </summary>
    [SerializeField]
    private bool hasTrail;

    /// <summary>
    /// Gives read-only access to the field: isDragging
    /// </summary>
    public bool IsDragging
    {
        get { return isDragging;  }
    }

    // Start is called before the first frame update
    void Start()
    {
        spriteRen = gameObject.GetComponent<SpriteRenderer>();

        if (hasTrail)
        {
            // Disenable trail renderer when first started
            trailRen = gameObject.GetComponent<TrailRenderer>();
            trailRen.enabled = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnDragging(InputAction.CallbackContext context)
    {
        // Dragging
        if (context.performed)
        {
            // Update color/trail if sprite/trail render exists
            if (spriteRen != null)
            {
                spriteRen.color = Color.yellow;
            }
            if (hasTrail)
            {
                trailRen.enabled = true;
            }

            // Update dragging status
            isDragging = true;

            return;     // Skip rest of the method
        }

        // Not dragging
        // Update color/trail if sprite/trail render exists
        if (spriteRen != null)
        {
            spriteRen.color = Color.white;
        }
        if (hasTrail)
        {
            trailRen.enabled = false;
        }

        // Update dragging status
        isDragging = false;
    }
}
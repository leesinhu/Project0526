using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFoot : MonoBehaviour
{
    private int groundContacts = 0;
    PlayerMovement player;

    private void Awake()
    {
        player = GetComponentInParent<PlayerMovement>();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            groundContacts++;
        }
            
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            groundContacts--;
        }
    }

    void FixedUpdate()
    {
        player.isGrounded = groundContacts > 0;
    }
}

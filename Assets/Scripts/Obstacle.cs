using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstacle : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.layer == LayerMask.NameToLayer("Controllable"))
        {
            PlayerMovement unit = collision.GetComponent<PlayerMovement>();
            if(unit != null)
            {
                unit.enabled = false;
                unit.Die();
                this.gameObject.SetActive(false);
            }
        }
    }
}

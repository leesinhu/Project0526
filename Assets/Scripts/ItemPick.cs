using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemPick : MonoBehaviour
{
    public bool isArrow;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            if (isArrow)
                collision.gameObject.GetComponent<PlayerMovement>().hasArrow = true;
            else
                collision.gameObject.GetComponent<PlayerMovement>().hasTorch = true;

            Destroy(gameObject);
        }
    }
}

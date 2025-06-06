using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemPick : MonoBehaviour
{
    public bool isArrow;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Player" || collision.gameObject.tag == "Mimic")
        {
            if (isArrow)
            {
                try
                {
                    if (collision.gameObject.tag == "Player")
                        GameObject.FindGameObjectWithTag("Mimic").GetComponent<PlayerMovement>().hasArrow = true;
                    else
                        GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>().hasArrow = true;
                }
                catch { }

                collision.gameObject.GetComponent<PlayerMovement>().hasArrow = true;
                GameManager.Instance.UpdateArrow(true);
            }
            Destroy(gameObject);
        }
    }
}

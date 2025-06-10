using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemPick : MonoBehaviour
{
    public bool isArrow;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player" || collision.gameObject.tag == "Mimic")
        {
            if (isArrow)
            {
                /*try
                {
                    if (collision.gameObject.tag == "Player")
                        GameObject.FindGameObjectWithTag("Mimic").GetComponent<PlayerMovement>().hasArrow = true;
                    else
                        GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>().hasArrow = true;
                }
                catch { }*/

                SoundManager.Instance.PrintSoundEffect("ready");
                collision.gameObject.GetComponent<PlayerMovement>().PickArrow(true);
                //GameManager.Instance.UpdateArrow(true);
            }
            gameObject.SetActive(false);
        }
    }
}

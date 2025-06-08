using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sign : MonoBehaviour
{
    [TextArea(3, 10)]
    [SerializeField] private string sign_ment = "null";
    [SerializeField] private Transform uiPosition;


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player" || collision.gameObject.tag == "Mimic")
        {
            GameManager.Instance.ShowSignUI(sign_ment, uiPosition, true);
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        GameManager.Instance.UpdateSignUI(uiPosition);
    }


    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player" || collision.gameObject.tag == "Mimic")
        {
            GameManager.Instance.ShowSignUI(sign_ment, uiPosition, false);
        }
    }
}

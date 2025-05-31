using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow : MonoBehaviour
{
    public float speed = 10f;
    private Vector3 moveDirection;

    private Rigidbody2D rb;

    public void SetArrow(bool is_right)
    {
        if (is_right)
            moveDirection = Vector3.right;
        else
            moveDirection = Vector3.left;

        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (moveDirection != null)
            rb.velocity = moveDirection * speed;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Destroy(gameObject);
    }

}

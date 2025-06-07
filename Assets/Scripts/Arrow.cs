using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow : MonoBehaviour
{
    public float speed = 13f;
    private Vector3 moveDirection;

    private Rigidbody2D rb;

    private Vector3 startPosition;
    public float maxDistance = 20f;

    public void SetArrow(bool is_right)
    {
        if (is_right)
        {
            moveDirection = Vector3.right;
        }
        else
        {
            moveDirection = Vector3.left;
            GetComponent<SpriteRenderer>().flipX = true;
        }

        rb = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        startPosition = transform.position;
    }
    void Update()
    {
        if (moveDirection != null)
            rb.velocity = moveDirection * speed;

        if (Vector3.Distance(startPosition, transform.position) > maxDistance)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player" || collision.gameObject.tag == "Mimic")
        {
            collision.gameObject.GetComponent<PlayerMovement>().Die();
            Destroy(gameObject);
        }
        else if(collision.gameObject.tag == "Waterfall" || collision.gameObject.tag == "IceWall"
            || collision.gameObject.layer == LayerMask.NameToLayer("Ground") || collision.gameObject.CompareTag("Enemy"))
        {
            Destroy(gameObject);
        }
    }
}

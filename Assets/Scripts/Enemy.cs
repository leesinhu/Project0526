using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float detectionRange = 5f;
    public float moveSpeed = 2f;
    public string[] targetTags = { "Player", "Mimic" };

    private Transform target;
    private Rigidbody2D rb;
    private bool isChasing = false; // ���� ����
    public Vector3 startPosition; // �Ŵ޸� ��ġ ����

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        startPosition = transform.position;
    }

    private void Update()
    {
        FindTarget();
        /*if(!isChasing)
        {
            FindTarget();
        }*/
    }

    // �÷��̾�, �н� ����
    private void FixedUpdate()
    {
        if(isChasing && target != null)
        {
            Vector2 direction = (target.position - transform.position).normalized;
            rb.MovePosition(rb.position + direction * moveSpeed * Time.fixedDeltaTime);
        }
    }

    // ���� �ȿ��� �÷��̾�, �н� Ž��
    void FindTarget()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, detectionRange);
        foreach(Collider2D hit in hits)
        {
            foreach(string tag in targetTags)
            {
                if(hit.CompareTag(tag))
                {
                    target = hit.transform;
                    isChasing = true;
                    return;
                }
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        GameObject other = collision.gameObject;

        // �÷��̾�, �н�, �������� �浹 ��
        if(other.CompareTag("Player") || other.CompareTag("Mimic") || other.name.Contains("Gate"))
        {
            transform.position = startPosition;
            isChasing = false;
            gameObject.SetActive(false);
        }

        // �̿� �浹 �� ����
        if (other.CompareTag("Untagged"))
        {
            Physics2D.IgnoreCollision(collision.collider, GetComponent<Collider2D>());
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //�������� ���� ��
        GameObject other = collision.gameObject;
        if(other.CompareTag("Waterfall") || other.CompareTag("Item" ))
        {
            transform.position = startPosition;
            isChasing = false;
            gameObject.SetActive(false);
        }
    }

    // �� �信�� ���� ������ ���������� ǥ��
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
    }
}

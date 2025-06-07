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
    private bool isChasing = false; // 추적 상태
    public Vector3 startPosition; // 매달린 위치 저장

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

    // 플레이어, 분신 추적
    private void FixedUpdate()
    {
        if(isChasing && target != null)
        {
            Vector2 direction = (target.position - transform.position).normalized;
            rb.MovePosition(rb.position + direction * moveSpeed * Time.fixedDeltaTime);
        }
    }

    // 범위 안에서 플레이어, 분신 탐지
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

        // 플레이어, 분신, 얼음벽과 충돌 시
        if(other.CompareTag("Player") || other.CompareTag("Mimic") || other.name.Contains("Gate"))
        {
            transform.position = startPosition;
            isChasing = false;
            gameObject.SetActive(false);
        }

        // 이외 충돌 시 무시
        if (other.CompareTag("Untagged"))
        {
            Physics2D.IgnoreCollision(collision.collider, GetComponent<Collider2D>());
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //폭포수와 접촉 시
        GameObject other = collision.gameObject;
        if(other.CompareTag("Waterfall") || other.CompareTag("Item" ))
        {
            transform.position = startPosition;
            isChasing = false;
            gameObject.SetActive(false);
        }
    }

    // 씬 뷰에서 감지 범위를 가시적으로 표현
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
    }
}

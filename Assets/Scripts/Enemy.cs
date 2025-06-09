using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public Vector2 detectionOffset = Vector2.zero;
    public float detectionRadiusX = 3f; // 가로 반지름
    public float detectionRadiusY = 5f; // 세로 반지름

    //public float detectionRange = 5f;
    public float moveSpeed = 2f;
    public string[] targetTags = { "Player", "Mimic" };

    private Transform target;
    private Rigidbody2D rb;
    private bool isChasing = false; // 추적 상태
    public Vector3 startPosition; // 매달린 위치 저장
    private Vector3 targetLastPosition;

    [SerializeField] private ParticleSystem effect;

    Animator anim;

    private void Start()
    {
        anim = GetComponent<Animator>();
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
        if (isChasing && target != null)
        {
            Vector3 chasePosition = target != null ? target.position : targetLastPosition;
            Vector2 direction = ((Vector2)chasePosition - rb.position).normalized;
            rb.MovePosition(rb.position + direction * moveSpeed * Time.fixedDeltaTime);
        }
    }

    // 범위 안에서 플레이어, 분신 탐지
    void FindTarget()
    {
        if (isChasing && target != null)
            return;

        Vector2 detectionCenter = (Vector2)transform.position + detectionOffset;

        Collider2D[] hits = Physics2D.OverlapCircleAll(detectionCenter, Mathf.Max(detectionRadiusX, detectionRadiusY));
        foreach (Collider2D hit in hits)
        {
            Vector2 diff = (Vector2)hit.transform.position - detectionCenter;
            float dx = diff.x / detectionRadiusX;
            float dy = diff.y / detectionRadiusY;

            if (dx * dx + dy * dy <= 1f)
            {
                foreach (string tag in targetTags)
                {
                    if (hit.CompareTag(tag))
                    {
                        target = hit.transform;
                        targetLastPosition = target.position;
                        isChasing = true;
                        anim.SetBool("isChasing", true);
                        return;
                    }
                }
            }
        }
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        GameObject other = collision.gameObject;

        // 충돌 후 리셋 및 비활성화 대상 태그/이름
        bool shouldReset =
            other.CompareTag("Player") ||
            other.CompareTag("Mimic") ||
            other.name.Contains("Gate") ||
            other.CompareTag("Waterfall") ||
            other.CompareTag("Item");

        if (shouldReset)
        {
            anim.SetBool("isChasing", false);
            isChasing = false;
            target = null; // 타겟도 해제

            effect.Play();
            GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0);
            Invoke("Die", 1f);
        }

        // 그 외 "Untagged" 충돌 무시
        /*if (other.CompareTag("Untagged"))
        {
            Physics2D.IgnoreCollision(collision, GetComponent<Collider2D>());
        }*/
    }

    private void Die()
    {
        transform.position = startPosition;
        gameObject.SetActive(false);
    }


    // 씬 뷰에서 감지 범위를 가시적으로 표현
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.matrix = transform.localToWorldMatrix;
        Gizmos.DrawWireCube(Vector3.zero, new Vector3(detectionRadiusX * 2, detectionRadiusY * 2, 0));
        Gizmos.DrawWireSphere(Vector3.zero, 1f); // 타원 대체 시각적 참조 (원)
    }

    public void ResetEnemy()
    {
        GetComponent<SpriteRenderer>().color = Color.white;
        anim.SetBool("isChasing", false);
        isChasing = false;
        target = null; // 타겟도 해제
        transform.position = startPosition;
    }

    private void OnEnable()
    {
        GetComponent<SpriteRenderer>().color = Color.white;
    }
}
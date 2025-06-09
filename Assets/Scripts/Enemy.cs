using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public Vector2 detectionOffset = Vector2.zero;
    public float detectionRadiusX = 3f; // ���� ������
    public float detectionRadiusY = 5f; // ���� ������

    //public float detectionRange = 5f;
    public float moveSpeed = 2f;
    public string[] targetTags = { "Player", "Mimic" };

    private Transform target;
    private Rigidbody2D rb;
    private bool isChasing = false; // ���� ����
    public Vector3 startPosition; // �Ŵ޸� ��ġ ����
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

    // �÷��̾�, �н� ����
    private void FixedUpdate()
    {
        if (isChasing && target != null)
        {
            Vector3 chasePosition = target != null ? target.position : targetLastPosition;
            Vector2 direction = ((Vector2)chasePosition - rb.position).normalized;
            rb.MovePosition(rb.position + direction * moveSpeed * Time.fixedDeltaTime);
        }
    }

    // ���� �ȿ��� �÷��̾�, �н� Ž��
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

        // �浹 �� ���� �� ��Ȱ��ȭ ��� �±�/�̸�
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
            target = null; // Ÿ�ٵ� ����

            effect.Play();
            GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0);
            Invoke("Die", 1f);
        }

        // �� �� "Untagged" �浹 ����
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


    // �� �信�� ���� ������ ���������� ǥ��
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.matrix = transform.localToWorldMatrix;
        Gizmos.DrawWireCube(Vector3.zero, new Vector3(detectionRadiusX * 2, detectionRadiusY * 2, 0));
        Gizmos.DrawWireSphere(Vector3.zero, 1f); // Ÿ�� ��ü �ð��� ���� (��)
    }

    public void ResetEnemy()
    {
        GetComponent<SpriteRenderer>().color = Color.white;
        anim.SetBool("isChasing", false);
        isChasing = false;
        target = null; // Ÿ�ٵ� ����
        transform.position = startPosition;
    }

    private void OnEnable()
    {
        GetComponent<SpriteRenderer>().color = Color.white;
    }
}
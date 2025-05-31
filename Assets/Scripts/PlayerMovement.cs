using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] bool isMimic = false;

    public float moveSpeed { get; set; } = 3.5f;
    public float jumpForce { get; set; } = 5f;
    Transform groundCheck;
    public LayerMask groundLayer;
    public Rigidbody2D rb { get; set; }
    public bool isGrounded { get; set; }

    [SerializeField] float coyoteTimeDuration = 0.15f;  // �ڿ��� Ÿ�� ���ӽð�
    [SerializeField] float jumpBufferDuration = 0.15f;  // �������� ���ӽð�
    float coyoteTimeCounter = 0f;
    float jumpBufferCounter = 0f;

    //������
    public bool hasArrow = false;
    public bool hasTorch = false;
    [SerializeField] private Arrow Arrow;

    public InputManager inputManager { get; set; }
    public float movement { get; set; }
    public bool jumpFlag { get; set; }

    public bool canMove;

    Animator anim;
    SpriteRenderer spriteRenderer;
    private void Awake()
    {
        groundCheck = transform.GetChild(0);
        anim = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }
    void Start()
    {
        this.moveSpeed = GameManager.Instance.moveSpeed;
        this.jumpForce = GameManager.Instance.jumpForce;

        inputManager = GameManager.Instance.inputManager;
        inputManager.OnInput += HandleInput;

        rb = GetComponent<Rigidbody2D>();
        GameManager.Instance.units.Add(this.gameObject);

        canMove = true;
        if(this.CompareTag("Mimic"))   // ���� ������Ʈ�� player���� mimic���� ����
        {
            isMimic = true;
        }
    }

    void Update()
    {
        this.moveSpeed = GameManager.Instance.moveSpeed;
        this.jumpForce = GameManager.Instance.jumpForce;
        /*       // ���� �Է�
               if (jumpFlag && isGrounded)
               {
                   jumpBufferCounter = jumpBufferDuration;
               }
               else
               {
                   jumpBufferCounter -= Time.deltaTime;
               }

               if (isGrounded)
               {
                   coyoteTimeCounter = coyoteTimeDuration; // ���� �� �ڿ��� Ÿ�� �ʱ�ȭ
               }
               else
               {
                   coyoteTimeCounter -= Time.deltaTime;  // ���߿� ������ ī���� ����
               }*/
        if (GameManager.Instance.screenLimit)
        {
            rb.velocity = new Vector2(0f, rb.velocity.y);
        }

        // ȭ�� �߻�
        if (hasArrow && Input.GetKeyDown(KeyCode.LeftShift))
        {
            Debug.Log("Arrow");
            Vector3 spawnPos = transform.position + new Vector3(spriteRenderer.flipX ? -1f : 1f, -0.5f, 0f);
            Instantiate(Arrow, spawnPos, Quaternion.identity).SetArrow(!spriteRenderer.flipX);
        }
    }

    private void FixedUpdate()
    {

        //
        //�߰�
        //�ִϸ��̼� �ۿ� 
        bool isWalking = movement != 0 && isGrounded;
        anim.SetBool("IsWalk", isWalking);
        //�̵��������� ��������Ʈ ȸ��
        if (movement != 0)
        {
            spriteRenderer.flipX = movement < 0; // �������� �̵� �� flipX = true
        }
        //


        if (jumpFlag && isGrounded && rb.velocity.y == 0)
        {
            rb.velocity = new Vector3(rb.velocity.x, 0f, 0f);
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            anim.SetTrigger("IsJump");
        }


        // �¿� �̵�
        if(canMove)
        { 
            if (isGrounded)
            {
                if(!GameManager.Instance.screenLimit)
                {
                    rb.velocity = new Vector3(movement * moveSpeed, rb.velocity.y, 0f);
                }
             
            }
            else
            {
                float currentX = rb.velocity.x;
                if (movement != 0 && Mathf.Sign(movement) != Mathf.Sign(currentX))
                {
                    float deceleration = 12.5f;
                    float newX = Mathf.MoveTowards(currentX, 0, deceleration * Time.fixedDeltaTime);
                    rb.velocity = new Vector3(newX, rb.velocity.y, 0f);
                }
            }
        }
    }

    private void HandleInput(object sender, InputEventArgs args)
    {
        switch (args.inputType)
        {
            case InputType.MoveLeft:
                if (!isMimic)
                {
                    movement = -1;
                }
                else
                {   //Mimic
                    float moveToOneFrame = (1 * moveSpeed) * Time.fixedDeltaTime;
                    float offset_x = transform.position.x + moveToOneFrame;
                    Vector2 temp = new Vector3(offset_x, transform.position.y);

                    Vector3 viewPos = Camera.main.WorldToViewportPoint(temp);
                    if (viewPos.x >= 0.9865f)
                    {
                        GameManager.Instance.screenLimit = true;
                        break;
                    }
                    else
                    {
                        GameManager.Instance.screenLimit = false;
                    }
                    movement = 1;
                }
                break;
            case InputType.MoveRight:
                if (!isMimic)
                {
                    movement = 1;
                }
                else
                {   //Mimic
                    float moveToOneFrame = (-1 * moveSpeed) * Time.fixedDeltaTime;
                    float offset_x = transform.position.x + moveToOneFrame;
                    Vector2 temp = new Vector3(offset_x, transform.position.y);

                    Vector3 viewPos = Camera.main.WorldToViewportPoint(temp);
                    if (viewPos.x <= 0.0135)
                    {
                        GameManager.Instance.screenLimit = true;
                        break;
                    }
                    else
                    {
                        GameManager.Instance.screenLimit = false;
                    }
                    movement = -1;
                }
                break;
            case InputType.MoveStop:
                movement = 0;
                break;
            case InputType.Jump:
                jumpFlag = true;
                break;
            case InputType.JumpEnd:
                jumpFlag = false;
                break;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Waterfall")
            rb.gravityScale = 2;

        //���̺� ����Ʈ ���� �� ����
        if (collision.CompareTag("Respawn"))
        {
            Debug.Log("���̺�����Ʈ ����!");
            GameManager.Instance.lastSpawnPoint = collision.gameObject.transform;
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Waterfall")
            rb.gravityScale = 1;
    }

    public void Die()
    {
        if (!isMimic)
        {
            //��ü(player + ��� mimic)
            GameManager.Instance.DestroyObj();
        }
        else
        {
            //�ش� mimic��
            GameManager.Instance.DestroyObj(this.gameObject);
        }
    }

    private void OnDestroy()
    {
        if (inputManager != null)
        {
            inputManager.OnInput -= HandleInput;
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        // �÷��̾�/�н��� ���� ���� ��� �н�/�÷��̾��� ������ ����
        if (collision.gameObject.layer == LayerMask.NameToLayer("Wall") && !isMimic) // player�� wall�� �������� ���
        {
            GameObject mimic = GameObject.FindGameObjectWithTag("Mimic");
            if (mimic != null)
            {
                var mimicMovement = mimic.GetComponent<PlayerMovement>();
                if ((mimicMovement != null))
                {
                    mimicMovement.rb.velocity = Vector2.zero;
                    mimicMovement.canMove = false;
                }
            }
        }
        else if (collision.gameObject.layer == LayerMask.NameToLayer("Wall") && isMimic) // mimic�� wall�� �������� ���
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                var playerMovement = player.GetComponent<PlayerMovement>();
                if (playerMovement != null)
                {
                    playerMovement.rb.velocity = Vector2.zero;
                    playerMovement.canMove = false;
                }
            }
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        // ������ ������ ��� ���� ����
        if (collision.gameObject.layer == LayerMask.NameToLayer("Wall") && !isMimic) // player�� wall�� �������� ���
        {
            GameObject mimic = GameObject.FindGameObjectWithTag("Mimic");
            if (mimic != null)
            {
                var mimicMovement = mimic.GetComponent<PlayerMovement>();
                if ((mimicMovement != null))
                {
                    mimicMovement.canMove = true;
                }
            }
        }
        else if (collision.gameObject.layer == LayerMask.NameToLayer("Wall") && isMimic) // mimic�� wall�� �������� ���
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                var playerMovement = player.GetComponent<PlayerMovement>();
                if (playerMovement != null)
                {
                    playerMovement.canMove = true;
                }
            }
        }
    }

}

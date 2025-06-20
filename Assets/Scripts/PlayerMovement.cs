using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] bool isMimic = false;
    public bool isReverse = false;

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
    [SerializeField] private GameObject Arrow;
    GameObject obj_arrow;

    [SerializeField] private GameObject effectPrefab;

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
        obj_arrow = transform.GetChild(1).gameObject;
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
        if (this.CompareTag("Mimic"))   // ���� ������Ʈ�� player���� mimic���� ����
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
                   coyoteTimeCounter = coyoteTimeDuration; 
               }
               else
               {
                   coyoteTimeCounter -= Time.deltaTime; 
               }*/
        if (GameManager.Instance.screenLimit)
        {
            rb.velocity = new Vector2(0f, rb.velocity.y);
        }


        if (hasArrow && Input.GetKeyDown(KeyCode.A))
        {
            SoundManager.Instance.PrintSoundEffect("throw");
            anim.SetTrigger("IsThrow");
            Vector3 spawnPos = transform.position + new Vector3(spriteRenderer.flipX ? -1f : 1f, 0.3f, 0f);
            var arrowInstance = Instantiate(Arrow, spawnPos, Quaternion.identity);
            arrowInstance.GetComponent<Arrow>().SetArrow(!spriteRenderer.flipX);
            PickArrow(false);
        }
    }

    private void FixedUpdate()
    {
        bool isWalking = movement != 0 && isGrounded;
        anim.SetBool("IsWalk", isWalking);
        if (movement != 0)
        {
            spriteRenderer.flipX = movement < 0; //
        }

        if (hasArrow)
        {
            if (spriteRenderer.flipX == false) //Right
            {
                obj_arrow.transform.localPosition = new Vector2(0.25f, 0.15f);
                obj_arrow.transform.rotation = Quaternion.Euler(0, 0, 60f);
            }
            else
            {
                obj_arrow.transform.localPosition = new Vector2(-0.25f, 0.15f);
                obj_arrow.transform.rotation = Quaternion.Euler(0, 0, 120f);
            }
        }

        if (jumpFlag && isGrounded && (rb.velocity.y < 0.1f && rb.velocity.y > -0.1f))
        {
            SoundManager.Instance.PrintSoundEffect("jump");
            rb.velocity = new Vector3(rb.velocity.x, 0f, 0f);
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            anim.SetTrigger("IsJump");
        }


        if (canMove)
        {
            if (isGrounded && (rb.velocity.y < 0.1f && rb.velocity.y > -0.1f))
            {
                if (!GameManager.Instance.screenLimit)
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

    public void PickArrow(bool b)
    {
        if (b)
        {
            hasArrow = true;
            obj_arrow.SetActive(true);
        }
        else
        {
            hasArrow = false;
            obj_arrow.SetActive(false);
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
                    if (isReverse) movement = 1;
                    else movement = -1;

                    float moveToOneFrame = (movement * moveSpeed) * Time.fixedDeltaTime;
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
                    ///movement = 1;
                }
                break;
            case InputType.MoveRight:
                if (!isMimic)
                {
                    movement = 1;
                }
                else
                {   //Mimic
                    if (isReverse) movement = -1;
                    else movement = 1;

                    float moveToOneFrame = (movement * moveSpeed) * Time.fixedDeltaTime;
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
                    //movement = -1;
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
        {
            rb.gravityScale = 10;
        }

        if (collision.CompareTag("Enemy"))
        {
            Die();
        }

        if (collision.CompareTag("Respawn"))
        {
            GameManager.Instance.SetNewSavePoint(collision.gameObject);
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Waterfall")
        {
            collision.gameObject.tag = "IceWall";
            rb.gravityScale = 1;
        }

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

    public void Die()
    {
        rb.velocity = Vector2.zero;

        if (!isMimic)
        {
            GameManager.Instance.DestroyObj();
        }
        else
        {
            GameManager.Instance.DestroyObj(this.gameObject);
        }
    }

    private void OnDestroy()
    {
        if (this.gameObject.tag == "Mimic")
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                var playerMovement = player.GetComponent<PlayerMovement>();
                playerMovement.canMove = true;
            }
        }

        if (inputManager != null)
        {
            inputManager.OnInput -= HandleInput;
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Wall") && !isMimic) // player
        {
            GameObject mimic = GameObject.FindGameObjectWithTag("Mimic");
            if (mimic != null && this.canMove)
            {
                var mimicMovement = mimic.GetComponent<PlayerMovement>();
                if ((mimicMovement != null))
                {
                    mimicMovement.rb.velocity = new Vector2(0f, mimicMovement.rb.velocity.y);
                    mimicMovement.canMove = false;
                }
            }
        }
        else if (collision.gameObject.layer == LayerMask.NameToLayer("Wall") && isMimic) // mimic
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                var playerMovement = player.GetComponent<PlayerMovement>();
                if (playerMovement != null && this.canMove)
                {
                    playerMovement.rb.velocity = new Vector2(0f, playerMovement.rb.velocity.y);
                    playerMovement.canMove = false;
                }
            }


        }
    }
}

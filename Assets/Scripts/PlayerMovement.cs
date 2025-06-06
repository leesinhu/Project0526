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

    [SerializeField] float coyoteTimeDuration = 0.15f;  // 코요테 타임 지속시간
    [SerializeField] float jumpBufferDuration = 0.15f;  // 점프버퍼 지속시간
    float coyoteTimeCounter = 0f;
    float jumpBufferCounter = 0f;

    //아이템
    public bool hasArrow = false;
    public bool hasTorch = false;
    [SerializeField] private GameObject Arrow;

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
        if(this.CompareTag("Mimic"))   // 현재 오브젝트가 player인지 mimic인지 구분
        {
            isMimic = true;
        }
    }

    void Update()
    {
        this.moveSpeed = GameManager.Instance.moveSpeed;
        this.jumpForce = GameManager.Instance.jumpForce;
        /*       // 점프 입력
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
                   coyoteTimeCounter = coyoteTimeDuration; // 착지 시 코요테 타임 초기화
               }
               else
               {
                   coyoteTimeCounter -= Time.deltaTime;  // 공중에 있으면 카운터 감소
               }*/
        if (GameManager.Instance.screenLimit)
        {
            rb.velocity = new Vector2(0f, rb.velocity.y);
        }

        // 화살 발사
        if (hasArrow && Input.GetKeyDown(KeyCode.LeftControl))
        {
            Vector3 spawnPos = transform.position + new Vector3(spriteRenderer.flipX ? -1f : 1f, 0.3f, 0f);
            var arrowInstance = Instantiate(Arrow, spawnPos, Quaternion.identity);
            arrowInstance.GetComponent<Arrow>().SetArrow(!spriteRenderer.flipX);
            hasArrow = false;
        }
    }

    private void FixedUpdate()
    {

        //
        //추가
        //애니메이션 작용 
        bool isWalking = movement != 0 && isGrounded;
        anim.SetBool("IsWalk", isWalking);
        //이동방향으로 스프라이트 회전
        if (movement != 0)
        {
            spriteRenderer.flipX = movement < 0; // 왼쪽으로 이동 시 flipX = true
        }
        //

        if (jumpFlag && isGrounded && (rb.velocity.y < 0.1f && rb.velocity.y > -0.1f))
        {
            rb.velocity = new Vector3(rb.velocity.x, 0f, 0f);
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            anim.SetTrigger("IsJump");
        }


        // 좌우 이동
        if(canMove)
        { 
            if (isGrounded && (rb.velocity.y < 0.1f && rb.velocity.y > -0.1f))
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
            rb.gravityScale = 10;

        //세이브 포인트 도달 시 갱신
        if (collision.CompareTag("Respawn"))
        {
            GameManager.Instance.lastSpawnPoint = collision.gameObject.transform;
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Waterfall")
        {
            collision.gameObject.tag = "Untagged";
            rb.gravityScale = 1;
        }

        // 벽에서 떨어진 경우 제한 해제
        if (collision.gameObject.layer == LayerMask.NameToLayer("Wall") && !isMimic) // player가 wall에 접촉중인 경우
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
        else if (collision.gameObject.layer == LayerMask.NameToLayer("Wall") && isMimic) // mimic이 wall에 접촉중인 경우
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
        if (!isMimic)
        {
            //전체(player + 모든 mimic)
            GameManager.Instance.DestroyObj();
        }
        else
        {
            //해당 mimic만
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

    private void OnTriggerStay2D(Collider2D collision)
    {
        // 플레이어/분신이 벽에 막힌 경우 분신/플레이어의 움직임 제한
        if (collision.gameObject.layer == LayerMask.NameToLayer("Wall") && !isMimic) // player가 wall에 접촉중인 경우
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
        else if (collision.gameObject.layer == LayerMask.NameToLayer("Wall") && isMimic) // mimic이 wall에 접촉중인 경우
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

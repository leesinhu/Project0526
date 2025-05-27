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

    public InputManager inputManager { get; set; }
    public float movement { get; set; }
    public bool jumpFlag { get; set; }


    private void Awake()
    {
        groundCheck = transform.GetChild(0);
    }
    void Start()
    {
        this.moveSpeed = GameManager.Instance.moveSpeed;
        this.jumpForce = GameManager.Instance.jumpForce;

        inputManager = GameManager.Instance.inputManager;
        inputManager.OnInput += HandleInput;

        rb = GetComponent<Rigidbody2D>();
        GameManager.Instance.units.Add(this.gameObject);
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
    }

    private void FixedUpdate()
    {
        if (jumpFlag && isGrounded && rb.velocity.y == 0)
        {
            rb.velocity = new Vector3(rb.velocity.x, 0f, 0f);
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        }

        // �¿� �̵�
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

    private void HandleInput(object sender, InputEventArgs args)
    {
        switch (args.inputType)
        {
            case InputType.MoveLeft:
                if(!isMimic)
                {   
                    movement = -1;
                }
                else
                {   //Mimic
                    float moveToOneFrame = (1 * moveSpeed) * Time.fixedDeltaTime;
                    float offset_x = transform.position.x + moveToOneFrame;
                    Vector2 temp = new Vector3(offset_x, transform.position.y);

                    Vector3 viewPos = Camera.main.WorldToViewportPoint(temp);
                    if(viewPos.x >= 0.9865f)
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

    public void Die()
    {
        if(!isMimic)
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
}

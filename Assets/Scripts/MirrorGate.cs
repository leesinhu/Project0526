using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MirrorGate : MonoBehaviour
{
    [SerializeField] GameObject mimic;
    Vector3 enterPosition;
    BoxCollider2D collider;
    SpriteRenderer spRend;

    public bool isSolid = false;

    [SerializeField] Sprite solidSprite;  //교체할 얼음 스프라이트
    Animator anim;                        // Animator
    Sprite _defaultSprite;                // 기본상태용 스프라이트
    private void Awake()
    {
        collider = GetComponent<BoxCollider2D>();
        spRend = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();

        _defaultSprite = spRend.sprite;
    }

    ///
    /// 얼음 이미지 적용 추가 
    ///
    private void Update()
    {
        if (isSolid)
        {
            // solid 상태일 때: 애니메이션 끄고 스프라이트 교체
            if (anim != null && anim.enabled)
                anim.enabled = false;

            if (spRend.sprite != solidSprite)
                spRend.sprite = solidSprite;
        }
        else
        {
            // non-solid 상태일 때: 애니메이션 키고 원래대로
            if (anim != null && !anim.enabled)
                anim.enabled = true;

            if (spRend.sprite != _defaultSprite)
                spRend.sprite = _defaultSprite;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            enterPosition = other.transform.position;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if(other.CompareTag("Player"))
        {
            PlayerMovement player = other.GetComponent<PlayerMovement>();
            float moveToOneFrame = (player.movement * player.moveSpeed) * Time.fixedDeltaTime;
            float offset_x = (other.transform.position.x + moveToOneFrame) - transform.position.x;
            Vector3 mimicPos = new Vector3(transform.position.x - offset_x, other.transform.position.y, 0);
            Instantiate(mimic, mimicPos, Quaternion.identity);

            ChangeState(1);
            //Destroy(this.gameObject);
        }
    }

    public void ChangeState(int i)
    {
        if(i == 1)
        {
            isSolid = true;
            collider.isTrigger = false;
            Color color = spRend.color;
            color.a = 1f;
            spRend.color = color;
        }
        else //0
        {
            isSolid = false;
            collider.isTrigger = true;
            Color color = spRend.color;
            color.a = 100f/255f;
            spRend.color = color;
        }
    }
}

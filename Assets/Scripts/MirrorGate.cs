using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MirrorGate : MonoBehaviour
{
    [SerializeField] GameObject mimic;
    [HideInInspector] public Transform playerPosition { get; set; }
    Vector3 enterPosition;
    BoxCollider2D collider;
    SpriteRenderer spRend;

    //Sound
    [SerializeField] AudioSource audio_waterfall;
    [SerializeField] AudioSource audio_ice;
    [SerializeField] [Range(0f, 0.75f)] float maxVolume = 0.75f;

    //ī�޶� �̵� ����
    [SerializeField] bool cameraChange;
    [SerializeField] Vector2 cameraChangePos;

    public bool isSolid = false;
    public float soundThreshold = 3f;

    private float distance;

    [SerializeField] Sprite solidSprite;
    Animator anim;                      
    Sprite _defaultSprite;
    private void Awake()
    {
        collider = GetComponent<BoxCollider2D>();
        spRend = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();

        _defaultSprite = spRend.sprite;
        audio_waterfall.volume = 0;
    }

    private void Update()
    {
        if (isSolid)
        {
            if (anim != null && anim.enabled)
                anim.enabled = false;

            if (spRend.sprite != solidSprite)
                spRend.sprite = solidSprite;
        }
        else
        {
            if (anim != null && !anim.enabled)
                anim.enabled = true;

            if (spRend.sprite != _defaultSprite)
                spRend.sprite = _defaultSprite;
        }

        if (playerPosition != null)
        {
            distance = Vector2.Distance(playerPosition.position, transform.position) - 4;

            if (isSolid || distance > soundThreshold)
            {
                audio_waterfall.volume = 0;
            }
            else
            {
                float t = Mathf.Clamp01(1f - (distance / soundThreshold));
                audio_waterfall.volume = Mathf.Pow(t, 2) * maxVolume;
            }
        }
        else
        {
            audio_waterfall.volume = 0;
        }
/*
        if (playerPosition != null)
        {
            distance = Vector2.Distance(playerPosition.position, transform.position) - 4;
        }
        if (isSolid || distance > soundThreshold)
            audio_waterfall.volume = 0;
        else
        {
            float t = Mathf.Clamp01(1f - (distance / soundThreshold));
            audio_waterfall.volume = Mathf.Pow(t, 2) * maxVolume;
            //audio_waterfall.volume = Mathf.Lerp(0, 1, Mathf.Clamp01(1f - (distance / soundThreshold)));
        }*/
            
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            enterPosition = other.transform.position;
        }

        // 얼음벽이 적과 닿으면 파괴됨
        if (other.gameObject.CompareTag("Enemy") && isSolid)
        {
            SoundManager.Instance.PrintSoundEffect("icebreak");
            this.gameObject.SetActive(false);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player") || other.CompareTag("Mimic"))
        {
            PlayerMovement unit = other.GetComponent<PlayerMovement>();
            float moveToOneFrame = (unit.movement * unit.moveSpeed) * Time.fixedDeltaTime;
            float offset_x = (other.transform.position.x + moveToOneFrame) - transform.position.x;
            Vector3 mimicPos = new Vector3(transform.position.x - offset_x, other.transform.position.y, 0);
            PlayerMovement _mimic = Instantiate(mimic, mimicPos, Quaternion.identity).GetComponent<PlayerMovement>();
            _mimic.isReverse = !unit.isReverse;
            _mimic.PickArrow(unit.hasArrow);

            audio_ice.Play();

            ChangeState(1);

            if(other.CompareTag("Player") && cameraChange)
            {
                CameraMovement camera = Camera.main.GetComponent<CameraMovement>();
                camera.StartCameraCutScene(cameraChangePos);
            }
            //Destroy(this.gameObject);
        }
    }

    public void ChangeState(int i)
    {
        if (i == 1)
        {
            isSolid = true;
            collider.isTrigger = false;
            Color color = spRend.color;
            color.a = 1f;
            spRend.color = color;

            //Wall
            transform.GetChild(0).gameObject.SetActive(true);
            transform.GetChild(1).gameObject.SetActive(true);
        }
        else //0
        {
            isSolid = false;
            collider.isTrigger = true;
            Color color = spRend.color;
            color.a = 175f / 255f;
            spRend.color = color;

            //Wall
            transform.GetChild(0).gameObject.SetActive(false);
            transform.GetChild(1).gameObject.SetActive(false);
        }
    }

}

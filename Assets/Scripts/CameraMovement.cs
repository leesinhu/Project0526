using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    Transform target;        // 따라갈 대상 (플레이어)
    public float followOffsetX = 2f; // X축 데드존 거리
    public float followOffsetY = 1.5f; // Y축 데드존 거리
    public float smoothSpeed = 5f;   // 따라가는 속도

    private void Awake()
    {
        target = GameObject.Find("Player").transform;
    }
    private void Start()
    {
       /* Vector3 cameraPos = transform.position;
        Vector3 targetPos = target.position;
        cameraPos.x = targetPos.x;
        float yDistance = targetPos.y - cameraPos.y;
        cameraPos.z = -10f;
        transform.position = cameraPos;*/
    }

    void Update()
    {
        if(target != null)
        {
            Vector3 cameraPos = transform.position;
            Vector3 targetPos = target.position;

            float xDistance = targetPos.x - cameraPos.x;
            if (Mathf.Abs(xDistance) > followOffsetX)
            {
                cameraPos.x = targetPos.x - followOffsetX * Mathf.Sign(xDistance);
            }

            float yDistance = targetPos.y - cameraPos.y;
            if (Mathf.Abs(yDistance) > followOffsetY)
            {
                cameraPos.y = Mathf.Lerp(cameraPos.y, targetPos.y, Time.deltaTime * smoothSpeed);
            }
            cameraPos.z = -10f;
            transform.position = cameraPos;
        }
        else
        {
            target = GameObject.FindWithTag("Player")?.transform;
        }
    }
}

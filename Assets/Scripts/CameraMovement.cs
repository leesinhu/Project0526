using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    Transform target;        // ���� ��� (�÷��̾�)
    public float followOffsetX = 2f; // X�� ������ �Ÿ�
    public float followOffsetY = 1.5f; // Y�� ������ �Ÿ�
    public float smoothSpeed = 5f;   // ���󰡴� �ӵ�

    public bool isCutScene;

    private void Awake()
    {
        target = GameObject.Find("Player").transform;
    }
    private void Start()
    {
        isCutScene = false;
        /* Vector3 cameraPos = transform.position;
         Vector3 targetPos = target.position;
         cameraPos.x = targetPos.x;
         float yDistance = targetPos.y - cameraPos.y;
         cameraPos.z = -10f;
         transform.position = cameraPos;*/
    }

    void Update()
    {
        // // �׽�Ʈ�ڵ�
        // if (Input.GetKeyDown(KeyCode.F1))
        // {
        //     StartCameraCutScene(new Vector3(12.8f, 0f, 0));
        // }

        if (isCutScene)
            return;

        if (target != null)
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

    void LateUpdate()
    {
        Vector3 pos = transform.position;
        pos.x = Mathf.Clamp(pos.x, -4, 1000);
        transform.position = pos;
    }

    public void StartCameraCutScene(Vector3 targetPosition)
    {
        targetPosition.z = -10;
        StartCoroutine(CameraCutScene(targetPosition));
    }

    private IEnumerator CameraCutScene(Vector3 targetPosition)
    {
        isCutScene = true;

        Vector3 cameraPos = transform.position;
        float elapsedTime = 0f;

        while (elapsedTime < 1f)
        {
            transform.position = Vector3.Lerp(cameraPos, targetPosition, elapsedTime / 1f);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // 0.7�� ��� �� ������ġ��
        yield return new WaitForSeconds(0.7f);
        transform.position = targetPosition;
        isCutScene = false;
    }
}

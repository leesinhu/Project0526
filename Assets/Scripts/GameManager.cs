using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    //Inspector
    public InputManager inputManager;
    public float moveSpeed;
    public float jumpForce;

    public Transform lastSpawnPoint;
    public int checkpointCount = 3;
    [SerializeField] Vector2 gravityScale;
    [SerializeField] GameObject player;

    //Flag
    public bool screenLimit { get; set; } = false; 
    public bool wallLimit { get; set; } = false;

    //List
    [HideInInspector] public List<GameObject> units { get; set; } = new List<GameObject>();
    [HideInInspector] public List<List<GameObject>> obstacles { get; set; } = new List<List<GameObject>>();
    [HideInInspector] public List<List<MirrorGate>> gates { get; set; } = new List<List<MirrorGate>>();
    public List<Transform> spawnPoints = new List<Transform>();

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject); 
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject); 

        lastSpawnPoint = spawnPoints[0]; 

        player = Resources.Load<GameObject>("Prefab/Player_Penguin");

        Physics2D.gravity = gravityScale;

        // 스폰 포인트에 따라 소속된 오브젝트를 obstacles, gates 2차원 구조에 초기화
        for (int i = 0; i < spawnPoints.Count; i++)
        {
            obstacles.Add(new List<GameObject>());

            string obsName = $"Obstacles{i + 1}";
            GameObject parentObj = GameObject.Find(obsName);

            Transform parent_obstacles = parentObj.transform;
            foreach (Transform child in parent_obstacles)
            {
                obstacles[i].Add(child.gameObject);
            }
        }
            /*gates.Add(new List<MirrorGate>());

            string gateName = $"Gates{i + 1}";
            GameObject parentObj2 = GameObject.Find(gateName);
            Transform parent_gates = parentObj2.transform;
            foreach (Transform gate in parent_gates)
            {
                MirrorGate temp = gate.GetComponent<MirrorGate>();
                gates[i].Add(temp);
                temp.playerPosition = GameObject.FindWithTag("Player").transform;
            }*/
        

        
    }

    private void Update()
    {
        Physics2D.gravity = gravityScale;
    }
    public void DestroyObj(GameObject targetObj = null)
    {
        if (targetObj != null)
        {
            Destroy(targetObj);
            units.Remove(targetObj);
        }
        else
        {
            Debug.Log("1");
            StartCoroutine(Respawn());
        }
    }

    IEnumerator Respawn()
    {
        GameObject newPlayer = player;
        foreach (GameObject obj in units)
        {
            Destroy(obj);
        }
        yield return null;
        units.Clear();

        yield return new WaitForSeconds(1f);
        Instantiate(newPlayer, lastSpawnPoint.position, Quaternion.identity); // 플레이어 리스폰

        
        int checkpointIndex = spawnPoints.IndexOf(lastSpawnPoint);

        for (int i = checkpointIndex; i < obstacles.Count; i++)
        {
            // 장애물 리스폰
            foreach (GameObject obj in obstacles[i])
            {
                if (!obj.activeSelf) obj.SetActive(true);
            }
        }  
            /*// 폭포수 리스폰
            foreach (MirrorGate gate in gates[i])
            {
                if (gate.isSolid) gate.ChangeState(0);
                gate.playerPosition = GameObject.FindWithTag("Player").transform;

            }*/
        

    }

    // 디버깅용 개발자 리스폰
    public void RespawnAtIndex(int index)
    {
        if (index < 0 || index >= spawnPoints.Count)
        {
            Debug.LogWarning("Invalid respawn index");
            return;
        }

        lastSpawnPoint = spawnPoints[index];
        // 기존 플레이어 및 유닛 제거 후 리스폰 시작
        StartCoroutine(Respawn());
    }

}

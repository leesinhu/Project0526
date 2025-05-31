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
    [SerializeField] Vector2 gravityScale;
    [SerializeField] GameObject player;

    //Flag
    public bool screenLimit { get; set; } = false; //�н��� ��ũ�� �Ѿ���� �� �� ����.
    public bool wallLimit { get; set; } = false;

    //List
    [HideInInspector] public List<GameObject> units { get; set; } = new List<GameObject>();
    [HideInInspector] public List<GameObject> obstacles { get; set; } = new List<GameObject>();
    [HideInInspector] public List<MirrorGate> gates { get; set; } = new List<MirrorGate>();
    public List<Transform> spawnPoints = new List<Transform>();

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject); // �ߺ� ����
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject); // �� �� ����

        lastSpawnPoint = spawnPoints[0]; // ù ���̺�����Ʈ ����

        player = Resources.Load<GameObject>("Prefab/Player_Penguin");

        Physics2D.gravity = gravityScale;

        Transform parent_obstacles = GameObject.Find("Obstacles").transform;
        foreach(Transform child in parent_obstacles)
        {
            obstacles.Add(child.gameObject);
        }

        Transform parent_gates = GameObject.Find("Gates").transform;
        foreach(Transform gate in parent_gates)
        {
            MirrorGate temp = gate.GetComponent<MirrorGate>();
            gates.Add(temp);
        }
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
        Instantiate(newPlayer, lastSpawnPoint.position, Quaternion.identity);
        foreach(GameObject obj in obstacles)
        {
            if (!obj.activeSelf) obj.SetActive(true);
        }
        foreach (MirrorGate gate in gates)
        {
            if (gate.isSolid) gate.ChangeState(0);
        }
    }
}

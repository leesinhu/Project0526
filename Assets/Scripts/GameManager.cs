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
    [SerializeField] Vector2 gravityScale;
    [SerializeField] GameObject player;

    //Flag
    public bool movePermit { get; set; } = true;

    //List
    public List<GameObject> units { get; set; } = new List<GameObject>();
    public List<Transform> spawnPoints = new List<Transform>();

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject); // 중복 제거
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject); // 씬 간 유지

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
        foreach (GameObject obj in units)
        {
            Destroy(obj);
        }
        yield return null;
        units.Clear();

        yield return new WaitForSeconds(1f);
        Instantiate(player, spawnPoints[0].position, Quaternion.identity);
    }
}

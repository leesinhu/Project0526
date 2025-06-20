using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    CameraMovement camera;

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
    public bool clear = false;

    // Canvas
    [SerializeField] GameObject signUI;

    //List
    [HideInInspector] public List<GameObject> units { get; set; } = new List<GameObject>();
    [HideInInspector] public List<List<GameObject>> obstacles { get; set; } = new List<List<GameObject>>();
    [HideInInspector] public List<List<MirrorGate>> gates { get; set; } = new List<List<MirrorGate>>();
    [HideInInspector] public List<List<GameObject>> enemies { get; set; } = new List<List<GameObject>>();
    [HideInInspector] public List<List<GameObject>> items { get; set; } = new List<List<GameObject>>();
    public List<Transform> spawnPoints = new List<Transform>();

    void Awake()
    {
        /*if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }*/

        Instance = this;
        //DontDestroyOnLoad(gameObject);

        lastSpawnPoint = spawnPoints[0];

        player = Resources.Load<GameObject>("Prefab/Player_Penguin");

        Physics2D.gravity = gravityScale;

        camera = Camera.main.GetComponent<CameraMovement>();
    }
    private void Start()
    {
        // 스폰 포인트에 따라 소속된 오브젝트를 obstacles, gates 2차원 구조에 초기화
        for (int i = 0; i < spawnPoints.Count; i++)
        {
            // 장애물
            obstacles.Add(new List<GameObject>());

            string obsName = $"Obstacles{i + 1}";
            GameObject parentObj = GameObject.Find(obsName);

            Transform parent_obstacles = parentObj.transform;
            foreach (Transform child in parent_obstacles)
            {
                obstacles[i].Add(child.gameObject);
            }

            // 폭포수
            gates.Add(new List<MirrorGate>());

            string gateName = $"Gates{i + 1}";
            GameObject parentObj2 = GameObject.Find(gateName);
            Transform parent_gates = parentObj2.transform;
            foreach (Transform gate in parent_gates)
            {
                MirrorGate temp = gate.GetComponent<MirrorGate>();
                gates[i].Add(temp);
                temp.playerPosition = GameObject.FindWithTag("Player").transform;
            }

            //몬스터
            enemies.Add(new List<GameObject>());

            string enName = $"Enemy{i + 1}";
            GameObject parentObj3 = GameObject.Find(enName);
            Transform parent_enemies = parentObj3.transform;
            foreach (Transform enemy in parent_enemies)
            {
                enemies[i].Add(enemy.gameObject);
            }


            // 아이템(화살)
            items.Add(new List<GameObject>());

            string itemName = $"Item{i + 1}";
            GameObject parentObj4 = GameObject.Find(itemName);
            Transform parent_items = parentObj4.transform;
            foreach (Transform item in parent_items)
            {
                items[i].Add(item.gameObject);
            }
        }

        SoundManager.Instance.PlaySoundTrack("main");
    }
    private void Update()
    {
        Physics2D.gravity = gravityScale;

        if (Input.GetKeyDown(KeyCode.R) && !clear)
        {
            StartCoroutine(Respawn());
        }
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

        for (int i = checkpointIndex; i < gates.Count; i++)
        {
            // 폭포수 리스폰
            foreach (MirrorGate gate in gates[i])
            {
                if (gate.gameObject.activeSelf == false) gate.gameObject.SetActive(true);
                if (gate.isSolid) gate.ChangeState(0);
                gate.gameObject.tag = "Waterfall";
                gate.playerPosition = GameObject.FindWithTag("Player").transform;
            }
        }

        for (int i = checkpointIndex; i < enemies.Count; i++)
        {
            // 적(박쥐) 리스폰
            foreach (GameObject obj in enemies[i])
            {
                Enemy enemy = obj.GetComponent<Enemy>();
                //enemy.transform.position = enemy.startPosition;
                if (!enemy.gameObject.activeSelf) enemy.gameObject.SetActive(true);
                enemy.ResetEnemy();
            }
        }

        for (int i = checkpointIndex; i < items.Count; i++)
        {
            // 아이템(화살) 리스폰
            foreach (GameObject obj in items[i])
            {
                if (!obj.activeSelf) obj.SetActive(true);
            }
        }

        screenLimit = false;
        wallLimit = false;
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


    public void ShowSignUI(string ment, Transform tr_sign, bool isActive)
    {
        if (!isActive)
        {
            signUI.SetActive(isActive);
            return;
        }
        signUI.GetComponent<RectTransform>().position = Camera.main.WorldToScreenPoint(tr_sign.position);
        signUI.SetActive(isActive);
        signUI.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = ment;
        LayoutRebuilder.ForceRebuildLayoutImmediate(signUI.GetComponent<RectTransform>());
    }

    public void UpdateSignUI(Transform tr_sign)
    {
        signUI.GetComponent<RectTransform>().position = Camera.main.WorldToScreenPoint(tr_sign.position);
    }

    public void BackToMain()
    {
        SoundManager.Instance.PlaySoundTrack("main");
        SceneManager.LoadScene("Title");
    }

    public void SetNewSavePoint(GameObject savePointObj)
    {
        GameManager.Instance.lastSpawnPoint = savePointObj.transform;
        int spawnPointIndex = -1;
        for(int i=0; i<spawnPoints.Count; i++)
        {
            if (spawnPoints[i] == savePointObj.transform)
            {
                spawnPointIndex = i;
                break;
            }
        }

        
        if (spawnPointIndex != -1)
        {

            if (spawnPointIndex >= 0 && spawnPointIndex <= 3)
            {
                SoundManager.Instance.PlaySoundTrack("vagabond", 0.25f);
            }
            else if (spawnPointIndex >= 4 && spawnPointIndex <= 11)
            {
                SoundManager.Instance.PlaySoundTrack("adventure", 0.35f);
            }
            else if (spawnPointIndex >= 12 && spawnPointIndex <= 19)
            {
                SoundManager.Instance.PlaySoundTrack("cave", 0.8f);
            }
            else if (spawnPointIndex >= 20 && spawnPointIndex <= 26)
            {
                if(spawnPointIndex == 20) camera.StartCameraCutScene(new Vector2(233.36f, -40.62513f));
                SoundManager.Instance.PlaySoundTrack("main", 0.5f);
            }
            else if (spawnPointIndex == 27)
            {
                SoundManager.Instance.PlaySoundTrack("mute", 0);
            }
            else if (spawnPointIndex == 28)
            {
                SoundManager.Instance.PlaySoundTrack("sunny", 0.7f, 5f);
                camera.StartCameraCutScene(new Vector2(348.38f, -17.66195f));
            }
            else if (spawnPointIndex == 29)
            {
                clear = true;
                camera.StartCameraCutScene(new Vector2(379.790009f, -19.6145744f), 5);
                GameObject playerInstance = GameObject.FindWithTag("Player");
                if (playerInstance != null)
                    Destroy(playerInstance);
            }
        }
        
    }
}

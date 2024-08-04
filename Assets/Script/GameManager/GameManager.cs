using System.Collections;
using UnityEngine;
using UnityEngine.TextCore.Text;
using UnityEngine.UIElements;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public SaveLoadManager saveLoadManager;
    public Vector3 playerPosition;

    private Vector3 savedCameraPosition;
    private GameObject blockPlayer;
    private Vector3 playerBlockerPos;

    public CheckpointManager checkpointManager;

    public PlayerController player;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        checkpointManager = FindObjectOfType<CheckpointManager>();

        player = FindObjectOfType<PlayerController>();

    }

    private void Start()
    {
        blockPlayer = GameObject.FindWithTag("PlayerBlocker");

        checkpointManager.CheckpointSave += Save;

        player.RespawnSystemAction += Load;

        Load();
    }

    private void OnDisable()
    {
        Debug.Log("GameManager取消订阅checkpoint.CheckpointSave");

        checkpointManager.CheckpointSave -= Save;

        player.RespawnSystemAction -= Load;
    }

    public void Save()
    {
        SaveCameraPosition(Camera.main.transform.position);
        SavePlayerPosition(player.transform.position);
        SavePlayerBlockerPos(blockPlayer.transform.position);

        PlayerPrefs.SetInt("CollectedGoals", GoalManager.instance.GetCollectedGoals());

        PlayerPrefs.SetInt("IsPlayerInverted", player.isInverted ? 1 : 0);

        PlayerPrefs.Save();
        Debug.Log("DateSave");
    }

    public void Load()
    {
        Camera.main.transform.position = LoadSavedCameraPosition();
        player.transform.position = LoadPlayerPosition();
        blockPlayer.transform.position = LoadPlayerBlockerPos();

        Debug.Log(PlayerPrefs.GetInt("IsPlayerInverted"));
        player.ResetInverted();

        GoalManager.instance.SetCollectedGoals(PlayerPrefs.GetInt("CollectedGoals", 0));
    }

    public void SavePlayerPosition(Vector3 position)
    {
        playerPosition = position;
        PlayerPrefs.SetFloat("PlayerX", position.x);
        PlayerPrefs.SetFloat("PlayerY", position.y);
        PlayerPrefs.SetFloat("PlayerZ", position.z);
        
    }

    public void SavePlayerBlockerPos(Vector3 position)
    {
        PlayerPrefs.SetFloat("BlockerX", blockPlayer.transform.position.x);
        PlayerPrefs.SetFloat("BlockerY", blockPlayer.transform.position.y);
        PlayerPrefs.SetFloat("BlockerZ", blockPlayer.transform.position.z);
    }

    public void SaveCameraPosition(Vector3 position)
    {
        PlayerPrefs.SetFloat("CameraPositionX", Camera.main.transform.position.x);
        PlayerPrefs.SetFloat("CameraPositionY", Camera.main.transform.position.y);
        PlayerPrefs.SetFloat("CameraPositionZ", Camera.main.transform.position.z);
    }

    public Vector3 LoadPlayerPosition()
    {
        float x = PlayerPrefs.GetFloat("PlayerX", -8);
        float y = PlayerPrefs.GetFloat("PlayerY", -4.7f);
        float z = PlayerPrefs.GetFloat("PlayerZ", 0);
        return new Vector3(x, y, z);
    }

    public Vector3 LoadPlayerBlockerPos()
    {
        float x = PlayerPrefs.GetFloat("BlockerX", 0);
        float y = PlayerPrefs.GetFloat("BlockerY", 0);
        float z = PlayerPrefs.GetFloat("BlockerZ", 0);
        return new Vector3(x, y, z);
    }

    public Vector3 LoadSavedCameraPosition()
    {
        float x = PlayerPrefs.GetFloat("CameraPositionX", 0);
        float y = PlayerPrefs.GetFloat("CameraPositionY", 0);
        float z = PlayerPrefs.GetFloat("CameraPositionZ", -10);
        return new Vector3(x, y, z);
    }

    private void Update()
    {
        //if (Input.GetKeyDown(KeyCode.I))
        //{
        //    saveLoadManager.SaveGame();
        //}
        //if (Input.GetKeyDown(KeyCode.O))
        //{
        //    saveLoadManager.LoadGame();
        //}
        //if (Input.GetKeyDown(KeyCode.R))
        //{
        //    PlayerPrefs.DeleteAll();
        //    Debug.Log("PlayerPrefs.DeleteAll");
        //}
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            StartCoroutine(Quit());
        }
    }

    private IEnumerator Quit()
    {
        float startTime = Time.time;
        float quitTimeout = 3f; // 超时时间（秒）
        while (Time.time - startTime < quitTimeout)
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                Application.Quit();
                yield break;
            }
    }
}

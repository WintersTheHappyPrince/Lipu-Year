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
        blockPlayer = GameObject.Find("BlockPlayer");

        checkpointManager.CheckpointSave += Save;

        player.RespawnSystemAction += Load;

        Save();
    }

    private void OnDisable()
    {
        Debug.Log("GameManagerÈ¡Ïû¶©ÔÄcheckpoint.CheckpointSave");
        checkpointManager.CheckpointSave -= Save;
        player.RespawnSystemAction -= Load;
    }

    public void Save()
    {
        SaveCameraPosition(Camera.main.transform.position);
        SavePlayerPosition(player.transform.position);
        SavePlayerBlockerPos(blockPlayer.transform.position);
        Debug.Log("DateSave");
    }

    public void Load()
    {
        Camera.main.transform.position = LoadSavedCameraPosition();
        player.transform.position = LoadPlayerPosition();
        blockPlayer.transform.position = LoadPlayerBlockerPos();
    }

    public void SavePlayerPosition(Vector3 position)
    {
        playerPosition = position;
        PlayerPrefs.SetFloat("PlayerX", position.x);
        PlayerPrefs.SetFloat("PlayerY", position.y);
        PlayerPrefs.SetFloat("PlayerZ", position.z);
        PlayerPrefs.Save();
    }

    public void SavePlayerBlockerPos(Vector3 position)
    {
        PlayerPrefs.SetFloat("BlockerX", blockPlayer.transform.position.x);
        PlayerPrefs.SetFloat("BlockerY", blockPlayer.transform.position.y);
        PlayerPrefs.SetFloat("BlockerZ", blockPlayer.transform.position.z);
        PlayerPrefs.Save();
    }

    public void SaveCameraPosition(Vector3 position)
    {
        PlayerPrefs.SetFloat("CameraPositionX", Camera.main.transform.position.x);
        PlayerPrefs.SetFloat("CameraPositionY", Camera.main.transform.position.y);
        PlayerPrefs.SetFloat("CameraPositionZ", Camera.main.transform.position.z);
        PlayerPrefs.Save();
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
        float x = PlayerPrefs.GetFloat("CameraPositionX", Camera.main.transform.position.x);
        float y = PlayerPrefs.GetFloat("CameraPositionY", Camera.main.transform.position.y);
        float z = PlayerPrefs.GetFloat("CameraPositionZ", Camera.main.transform.position.z);
        return new Vector3(x, y, z);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            saveLoadManager.SaveGame();
        }
        if (Input.GetKeyDown(KeyCode.O))
        {
            saveLoadManager.LoadGame();
        }
    }
}

using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public static PlayerManager instance;

    public PlayerController player;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        // Assuming PlayerController is attached to the same GameObject
        player = GetComponent<PlayerController>();

        // Alternatively, find the PlayerController in the scene
        if (player == null)
        {
            player = FindObjectOfType<PlayerController>();
        }

        if (player == null)
        {
            Debug.LogError("PlayerController not found!");
        }
    }
}

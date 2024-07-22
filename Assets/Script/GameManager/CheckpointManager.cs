using System.Collections.Generic;
using UnityEngine;

public class CheckpointManager : MonoBehaviour
{
    public static CheckpointManager instance;

    public Checkpoint currentCheckpoint; // ��ǰ����ļ���

    public System.Action CheckpointSave;

    private void Awake()
    {
        // Singleton pattern to ensure only one instance of CheckpointManager
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // ��֤ CheckpointManager �����ڳ����л�ʱ������
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SetActiveCheckpoint(Checkpoint checkpoint)
    {
        CheckpointSave?.Invoke();
        if (currentCheckpoint != null)
        {
            currentCheckpoint.SetState(Checkpoint.CheckpointState.Default);
        }

        currentCheckpoint = checkpoint;
        currentCheckpoint.SetState(Checkpoint.CheckpointState.Active);
    }

    public Checkpoint GetCurrentCheckpoint()
    {
        return currentCheckpoint;
    }

}
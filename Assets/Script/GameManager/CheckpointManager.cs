using System.Collections.Generic;
using UnityEngine;

public class CheckpointManager : MonoBehaviour
{
    public static CheckpointManager instance;

    public Checkpoint currentCheckpoint; // 当前激活的检查点
    public Vector3 lastCheckpointPosition; //最后激活的检查点位置

    public System.Action CheckpointSave;

    private void Awake()
    {
        // Singleton pattern to ensure only one instance of CheckpointManager
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SetActiveCheckpoint(Checkpoint checkpoint)
    {
        if (currentCheckpoint != null)
        {
            currentCheckpoint.SetState(Checkpoint.CheckpointState.Default);
        }

        currentCheckpoint = checkpoint;
        currentCheckpoint.SetState(Checkpoint.CheckpointState.Active);

        // 保存最后激活的检查点位置
        lastCheckpointPosition = currentCheckpoint.transform.position;

        CheckpointSave?.Invoke();
    }

    public Checkpoint GetCurrentCheckpoint()
    {
        return currentCheckpoint;
    }

}
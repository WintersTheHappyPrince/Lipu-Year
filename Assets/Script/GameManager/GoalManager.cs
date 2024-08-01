using UnityEngine;
using System.Collections.Generic;

public class GoalManager : MonoBehaviour
{
    public static GoalManager instance;

    private int collectedGoals = 0;
    private int totalGoals = 29; // 总共29个金币

    public GameManager gameManager;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void CollectGoal()
    {
        collectedGoals++;
        Debug.Log("Collected Goals: " + collectedGoals);

        if (collectedGoals == 28)
        {
            UnlockFinalLock();
        }
        else if (collectedGoals == totalGoals)
        {
            CompleteGame();
        }

        // 更新地图显示
        UpdateMapIndicators();

        // 保存游戏状态
        gameManager.Save();
    }

    private void UpdateMapIndicators()
    {
        // 获取所有目标金币的 GoalController 实例
        GoalController[] goalControllers = FindObjectsOfType<GoalController>();
        MapManager.instance.UpdateMapIndicators(collectedGoals, goalControllers);
    }

    private void UnlockFinalLock()
    {
        Debug.Log("Final lock unlocked!");
        // 在这里添加打开最后一把锁的逻辑
    }

    private void CompleteGame()
    {
        Debug.Log("Game completed!");
        // 在这里添加通关的逻辑
    }

    public int GetCollectedGoals()
    {
        return collectedGoals;
    }

    public void SetCollectedGoals(int value)
    {
        collectedGoals = value;
    }
}

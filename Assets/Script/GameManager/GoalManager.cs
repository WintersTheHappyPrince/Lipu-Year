using UnityEngine;
using System.Collections.Generic;

public class GoalManager : MonoBehaviour
{
    public static GoalManager instance;

    private int collectedGoals = 0;
    private int totalGoals = 29; // �ܹ�29�����

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

        // ���µ�ͼ��ʾ
        UpdateMapIndicators();

        // ������Ϸ״̬
        gameManager.Save();
    }

    private void UpdateMapIndicators()
    {
        // ��ȡ����Ŀ���ҵ� GoalController ʵ��
        GoalController[] goalControllers = FindObjectsOfType<GoalController>();
        MapManager.instance.UpdateMapIndicators(collectedGoals, goalControllers);
    }

    private void UnlockFinalLock()
    {
        Debug.Log("Final lock unlocked!");
        // ��������Ӵ����һ�������߼�
    }

    private void CompleteGame()
    {
        Debug.Log("Game completed!");
        // ���������ͨ�ص��߼�
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

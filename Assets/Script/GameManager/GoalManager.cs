using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class GoalManager : MonoBehaviour
{
    public static GoalManager instance;

    private int collectedGoals = 0;
    private int totalGoals = 29; // �ܹ�29�����

    public GameManager gameManager;
    public TransitionEffect transitionEffect;

    public System.Action CollectGoalSA;

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
        collectedGoals = PlayerPrefs.GetInt("CollectedGoals", 0);

        if (collectedGoals == 28)
        {
            UnlockFinalLock();
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

        // ������Ϸ״̬
        gameManager.Save();

        // ���µ�ͼ��ʾ
        //UpdateMapIndicators();

        // ������ʾ��
        CollectGoalSA?.Invoke();
    }

    private void UpdateMapIndicators()
    {
        // ��ȡ����Ŀ���ҵ� GoalController ʵ��
        GoalController[] goalControllers = FindObjectsOfType<GoalController>();
        //MapManager.instance.UpdateMapIndicators(collectedGoals, goalControllers);
    }

    private void UnlockFinalLock()
    {
        Debug.Log("Final lock unlocked!");
        GameObject finalLock = GameObject.FindWithTag("Lock");
        finalLock?.SetActive(false);
    }

    private IEnumerator CompleteGame()
    {
        Debug.Log("Game completed!");
        // ���������ͨ�ص��߼�
        PlayerManager.instance.player.enabled = false;
        transitionEffect.transitionSpeed = 0.5f;
        transitionEffect.PlayerDied();
        yield return new WaitForSeconds(0.1f);
        while (!transitionEffect.isTransitioning)
        {
            transitionEffect.transitionSpeed = 2f;
            yield return null;
        }
    }

    public int GetCollectedGoals()
    {
        return collectedGoals;
    }

    public void SetCollectedGoals(int value)
    {
        collectedGoals = value;
    }

    public bool HasSavedData()
    {
        if (GetCollectedGoals() > 0)
            return true;
        else
            return false;
    }
}

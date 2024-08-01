using UnityEngine;

public class MapManager : MonoBehaviour
{
    public static MapManager instance;

    public GameObject map; // ��ͼ��GameObject
    public GameObject goalIndicatorPrefab; // Ŀ�����ڵ�ͼ�ϵ�ָʾ��Prefab

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

    // ���µ�ͼ��Ŀ���ҵ���ʾ״̬
    public void UpdateMapIndicators(int collectedGoals, GoalController[] goalControllers)
    {
        // �������ָʾ��
        foreach (Transform child in map.transform)
        {
            Destroy(child.gameObject);
        }

        foreach (GoalController goalController in goalControllers)
        {
            // �ж�Ŀ�����Ƿ����ռ�
            if (goalController.goalGold.activeSelf)
            {
                // ʵ����Ŀ���ҵ�ָʾ��
                GameObject indicator = Instantiate(goalIndicatorPrefab, map.transform);
                // ����ָʾ��λ�ã���Ҫ����ʵ��������е�����
                indicator.transform.position = goalController.transform.position; // �����ͼ����Ϸ��������һ��
            }
        }
    }
}

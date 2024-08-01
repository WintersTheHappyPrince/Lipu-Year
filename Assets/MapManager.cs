using UnityEngine;

public class MapManager : MonoBehaviour
{
    public static MapManager instance;

    public GameObject map; // 地图的GameObject
    public GameObject goalIndicatorPrefab; // 目标金币在地图上的指示器Prefab

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

    // 更新地图上目标金币的显示状态
    public void UpdateMapIndicators(int collectedGoals, GoalController[] goalControllers)
    {
        // 清除现有指示器
        foreach (Transform child in map.transform)
        {
            Destroy(child.gameObject);
        }

        foreach (GoalController goalController in goalControllers)
        {
            // 判断目标金币是否已收集
            if (goalController.goalGold.activeSelf)
            {
                // 实例化目标金币的指示器
                GameObject indicator = Instantiate(goalIndicatorPrefab, map.transform);
                // 设置指示器位置（需要根据实际情况进行调整）
                indicator.transform.position = goalController.transform.position; // 假设地图和游戏世界坐标一致
            }
        }
    }
}

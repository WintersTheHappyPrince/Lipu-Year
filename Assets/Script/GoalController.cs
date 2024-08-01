using UnityEngine;

public class GoalController : MonoBehaviour
{
    public GameObject goalGold; // 转动的金币动画
    public GameObject checkPoint; // 未启用的检查点
    public SpriteRenderer numberRenderer; // 数字的SpriteRenderer

    private Color[] rainbowColors = new Color[29]; // 彩虹颜色数组

    private void Start()
    {
        // 初始化彩虹颜色
        InitializeRainbowColors();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        PlayerController player = collision.gameObject.GetComponent<PlayerController>();
        if (player != null)
        {
            CollectGoal();
        }
    }

    private void CollectGoal()
    {
        goalGold.SetActive(false); // 停用 goalGold
        checkPoint.SetActive(true); // 启用 checkPoint
        numberRenderer.color = GetRandomRainbowColor(); // 更改 Number 的颜色

        // 更新收集进度
        GoalManager.instance.CollectGoal();
    }

    private void InitializeRainbowColors()
    {
        // 初始化彩虹颜色，实际可以根据需要调整
        for (int i = 0; i < 29; i++)
        {
            rainbowColors[i] = Color.HSVToRGB(i / 29f, 1, 1);
        }
    }

    private Color GetRandomRainbowColor()
    {
        return rainbowColors[Random.Range(0, rainbowColors.Length)];
    }
}

using UnityEngine;

public class GoalController : MonoBehaviour
{
    public GameObject goalGold; // 转动的金币动画
    public GameObject checkPoint; // 未启用的检查点
    public SpriteRenderer[] numberRenderers; // 数字的SpriteRenderer

    private Color[] rainbowColors = new Color[29]; // 彩虹颜色数组

    private void Start()
    {
        // 初始化彩虹颜色
        InitializeRainbowColors();

        // 检查 PlayerPrefs 中的收集状态并切换状态
        if (PlayerPrefs.GetInt(gameObject.name, 0) == 1) // 0 表示未收集
        {
            // 如果已收集，禁用 Collider2D
            Collider2D collider = GetComponent<Collider2D>();
            if (collider != null)
            {
                collider.enabled = false;
            }

            // 停用 goalGold 并启用 checkPoint
            if (goalGold != null) goalGold.SetActive(false);
            if (checkPoint != null) checkPoint.SetActive(true);
        }
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

        Color currentColor = GetRandomRainbowColor();

        foreach (var numberRenderer in numberRenderers)
        {
            numberRenderer.color = currentColor; // 更改 Number 的颜色
        }

        // 记录收集状态
        // 获取当前颜色的索引并保存
        // 计算色相值 i
        float h, s, v;
        Color.RGBToHSV(currentColor, out h, out s, out v);
        int colorIndex = Mathf.RoundToInt(h * 29); // 将色相值转为 0 到 28 的整数

        // 保存色相值
        PlayerPrefs.SetInt(gameObject.name + "ColorIndex", colorIndex);

        // 更新收集进度
        GoalManager.instance.CollectGoal();
        PlayerPrefs.SetInt(gameObject.name, 1); // 保存收集状态
        PlayerPrefs.Save();

        // 禁用 Collider2D
        Collider2D collider = GetComponent<Collider2D>();
        if (collider != null)
        {
            collider.enabled = false;
        }
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

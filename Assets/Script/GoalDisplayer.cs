using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoalDisplayer : MonoBehaviour
{
    public GameObject goalGold; // 转动的金币动画

    public SpriteRenderer[] numberRenderers; // 数字的SpriteRenderer

    public bool invertDisplay;

    void Start()
    {
        GoalManager.instance.CollectGoalSA += UpdateDisplay;
        UpdateDisplay();
    }

    private void OnDisable()
    {
        GoalManager.instance.CollectGoalSA -= UpdateDisplay;
    }

    void UpdateDisplay()
    {
        // 获取目标金币的状态
        bool isCollected = PlayerPrefs.GetInt(gameObject.name, 0) == 1;

        // 根据 invertDisplay 的值决定显示的逻辑
        bool shouldDisplay = invertDisplay ? !isCollected : isCollected;

        if (shouldDisplay)
        {
            if (goalGold != null) goalGold.SetActive(true);

            Color currentColor = Color.HSVToRGB(PlayerPrefs.GetInt(gameObject.name + "ColorIndex") / 29f, 1, 1);

            if (numberRenderers != null)
            {
                foreach (var numberRenderer in numberRenderers)
                {
                    numberRenderer.enabled = true;
                    numberRenderer.color = currentColor; // 更改 Number 的颜色
                }
            }
        }
        else
        {
            if (goalGold != null) goalGold.SetActive(false);

            if (numberRenderers != null)
            {
                foreach (var numberRenderer in numberRenderers)
                {
                    numberRenderer.enabled = false;
                }
            }
        }
    }
}

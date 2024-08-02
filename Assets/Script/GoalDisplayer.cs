using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoalDisplayer : MonoBehaviour
{
    public GameObject goalGold; // 转动的金币动画

    public SpriteRenderer[] numberRenderers; // 数字的SpriteRenderer

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
        if (PlayerPrefs.GetInt(gameObject.name, 0) == 1) // 0 表示未收集
        {
            if (goalGold != null) goalGold.SetActive(true);

            Color currentColor = Color.HSVToRGB(PlayerPrefs.GetInt(gameObject.name + "ColorIndex") / 29f, 1, 1);

            foreach (var numberRenderer in numberRenderers)
            {
                numberRenderer.enabled = true;
                numberRenderer.color = currentColor; // 更改 Number 的颜色
            }
        }
        else
        {
            if (goalGold != null) goalGold.SetActive(false);
            foreach (var numberRenderer in numberRenderers)
            {
                numberRenderer.enabled = false;
            }
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoalDisplayer : MonoBehaviour
{
    public GameObject goalGold; // ת���Ľ�Ҷ���

    public SpriteRenderer[] numberRenderers; // ���ֵ�SpriteRenderer

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
        if (PlayerPrefs.GetInt(gameObject.name, 0) == 1) // 0 ��ʾδ�ռ�
        {
            if (goalGold != null) goalGold.SetActive(true);

            Color currentColor = Color.HSVToRGB(PlayerPrefs.GetInt(gameObject.name + "ColorIndex") / 29f, 1, 1);

            foreach (var numberRenderer in numberRenderers)
            {
                numberRenderer.enabled = true;
                numberRenderer.color = currentColor; // ���� Number ����ɫ
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

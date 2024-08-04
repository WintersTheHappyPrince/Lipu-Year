using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoalDisplayer : MonoBehaviour
{
    public GameObject goalGold; // ת���Ľ�Ҷ���

    public SpriteRenderer[] numberRenderers; // ���ֵ�SpriteRenderer

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
        // ��ȡĿ���ҵ�״̬
        bool isCollected = PlayerPrefs.GetInt(gameObject.name, 0) == 1;

        // ���� invertDisplay ��ֵ������ʾ���߼�
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
                    numberRenderer.color = currentColor; // ���� Number ����ɫ
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

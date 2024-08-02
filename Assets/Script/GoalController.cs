using UnityEngine;

public class GoalController : MonoBehaviour
{
    public GameObject goalGold; // ת���Ľ�Ҷ���
    public GameObject checkPoint; // δ���õļ���
    public SpriteRenderer[] numberRenderers; // ���ֵ�SpriteRenderer

    private Color[] rainbowColors = new Color[29]; // �ʺ���ɫ����

    private void Start()
    {
        // ��ʼ���ʺ���ɫ
        InitializeRainbowColors();

        // ��� PlayerPrefs �е��ռ�״̬���л�״̬
        if (PlayerPrefs.GetInt(gameObject.name, 0) == 1) // 0 ��ʾδ�ռ�
        {
            // ������ռ������� Collider2D
            Collider2D collider = GetComponent<Collider2D>();
            if (collider != null)
            {
                collider.enabled = false;
            }

            // ͣ�� goalGold ������ checkPoint
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
        goalGold.SetActive(false); // ͣ�� goalGold
        checkPoint.SetActive(true); // ���� checkPoint

        Color currentColor = GetRandomRainbowColor();

        foreach (var numberRenderer in numberRenderers)
        {
            numberRenderer.color = currentColor; // ���� Number ����ɫ
        }

        // ��¼�ռ�״̬
        // ��ȡ��ǰ��ɫ������������
        // ����ɫ��ֵ i
        float h, s, v;
        Color.RGBToHSV(currentColor, out h, out s, out v);
        int colorIndex = Mathf.RoundToInt(h * 29); // ��ɫ��ֵתΪ 0 �� 28 ������

        // ����ɫ��ֵ
        PlayerPrefs.SetInt(gameObject.name + "ColorIndex", colorIndex);

        // �����ռ�����
        GoalManager.instance.CollectGoal();
        PlayerPrefs.SetInt(gameObject.name, 1); // �����ռ�״̬
        PlayerPrefs.Save();

        // ���� Collider2D
        Collider2D collider = GetComponent<Collider2D>();
        if (collider != null)
        {
            collider.enabled = false;
        }
    }



    private void InitializeRainbowColors()
    {
        // ��ʼ���ʺ���ɫ��ʵ�ʿ��Ը�����Ҫ����
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

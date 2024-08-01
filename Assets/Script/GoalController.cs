using UnityEngine;

public class GoalController : MonoBehaviour
{
    public GameObject goalGold; // ת���Ľ�Ҷ���
    public GameObject checkPoint; // δ���õļ���
    public SpriteRenderer numberRenderer; // ���ֵ�SpriteRenderer

    private Color[] rainbowColors = new Color[29]; // �ʺ���ɫ����

    private void Start()
    {
        // ��ʼ���ʺ���ɫ
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
        goalGold.SetActive(false); // ͣ�� goalGold
        checkPoint.SetActive(true); // ���� checkPoint
        numberRenderer.color = GetRandomRainbowColor(); // ���� Number ����ɫ

        // �����ռ�����
        GoalManager.instance.CollectGoal();
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

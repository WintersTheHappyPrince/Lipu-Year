
using UnityEngine;
using UnityEngine.UI;

public class GridDrawer : MonoBehaviour
{
    public int gridWidth = 30;
    public int gridHeight = 16;
    public float cellSize = 100f; // 单位像素
    public Color gridColor = Color.black;

    private Image gridImage;

    void Start()
    {
        gridImage = GetComponent<Image>();
        gridImage.color = Color.white;
        DrawGrid();
        this.gameObject.SetActive(false);
    }

    void DrawGrid()
    {
        int width = Mathf.RoundToInt(gridWidth * cellSize);
        int height = Mathf.RoundToInt(gridHeight * cellSize);
        Texture2D texture = new Texture2D(width, height);

        // 填充背景颜色
        Color[] fillColor = new Color[width * height];
        for (int i = 0; i < fillColor.Length; i++)
            fillColor[i] = Color.clear; // 透明背景
        texture.SetPixels(fillColor);

        // 绘制网格线
        for (int x = 0; x <= width; x += Mathf.RoundToInt(cellSize))
        {
            for (int y = 0; y < height; y++)
                texture.SetPixel(x, y, gridColor);
        }
        for (int y = 0; y <= height; y += Mathf.RoundToInt(cellSize))
        {
            for (int x = 0; x < width; x++)
                texture.SetPixel(x, y, gridColor);
        }

        texture.Apply();
        gridImage.sprite = Sprite.Create(texture, new Rect(0, 0, width, height), new Vector2(0.5f, 0.5f));
    }
}

using UnityEngine;

public class BackgroundResizer : MonoBehaviour
{
    void Start()
    {
        ResizeBackground();
    }

    void ResizeBackground()
    {
        // 获取当前摄像机
        Camera cam = Camera.main;
        if (cam == null) return;

        // 获取背景图片的SpriteRenderer组件
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null) return;

        // 获取背景图片的原始大小
        Vector2 spriteSize = spriteRenderer.sprite.bounds.size;

        // 获取摄像机的视口大小
        float height = cam.orthographicSize * 2f;
        float width = height * cam.aspect;

        // 计算缩放比例
        Vector3 scale = transform.localScale;
        scale.x = width / spriteSize.x;
        scale.y = height / spriteSize.y;

        // 应用缩放比例
        transform.localScale = scale;
    }
}

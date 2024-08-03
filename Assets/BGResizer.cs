using UnityEngine;

public class BackgroundResizer : MonoBehaviour
{
    void Start()
    {
        ResizeBackground();
    }

    void ResizeBackground()
    {
        // ��ȡ��ǰ�����
        Camera cam = Camera.main;
        if (cam == null) return;

        // ��ȡ����ͼƬ��SpriteRenderer���
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null) return;

        // ��ȡ����ͼƬ��ԭʼ��С
        Vector2 spriteSize = spriteRenderer.sprite.bounds.size;

        // ��ȡ��������ӿڴ�С
        float height = cam.orthographicSize * 2f;
        float width = height * cam.aspect;

        // �������ű���
        Vector3 scale = transform.localScale;
        scale.x = width / spriteSize.x;
        scale.y = height / spriteSize.y;

        // Ӧ�����ű���
        transform.localScale = scale;
    }
}

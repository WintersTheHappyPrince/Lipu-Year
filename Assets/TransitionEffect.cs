using UnityEngine;
using UnityEngine.UI;

public class TransitionEffect : MonoBehaviour
{
    public GameObject player; // 引用玩家对象
    public GameObject circleMask; // 引用CircleMask对象
    public Canvas canvas; // 引用Transition的Canvas对象
    public float transitionSpeed = 2f; // 转场速度

    private RectTransform maskRectTransform;
    private bool isTransitioning = false;
    private bool isExpanding = false;

    void Start()
    {
        if (circleMask != null)
        {
            maskRectTransform = circleMask.GetComponent<RectTransform>();
        }
    }

    void Update()
    {
        if (isTransitioning)
        {
            if (isExpanding)
            {
                ExpandMask();
            }
            else
            {
                ShrinkMask();
            }
        }
    }

    public void PlayerDied()
    {
        isTransitioning = true;
        isExpanding = false;
        UpdateMaskPosition();
    }

    public void PlayerRespawn()
    {
        isTransitioning = true;
        isExpanding = true;
        UpdateMaskPosition();
    }

    private void ShrinkMask()
    {
        if (maskRectTransform == null)
        {
            Debug.LogError("maskRectTransform is not assigned.");
            return;
        }
        maskRectTransform.localScale -= Vector3.one * transitionSpeed * Time.deltaTime;
        if (maskRectTransform.localScale.x <= 0.1f)
        {
            maskRectTransform.localScale = Vector3.zero;
            isTransitioning = false;
            // 玩家死亡后其他逻辑
        }
    }

    private void ExpandMask()
    {
        if (maskRectTransform == null)
        {
            Debug.LogError("maskRectTransform is not assigned.");
            return;
        }
        maskRectTransform.localScale += Vector3.one * transitionSpeed * Time.deltaTime;
        if (maskRectTransform.localScale.x >= 1f)
        {
            maskRectTransform.localScale = Vector3.one;
            isTransitioning = false;
            // 玩家重生后其他逻辑
        }
    }

    private void UpdateMaskPosition()
    {
        if (player == null || maskRectTransform == null || canvas == null)
        {
            Debug.LogError("player, maskRectTransform or canvas is not assigned.");
            return;
        }

        // 获取玩家在屏幕上的位置
        Vector3 screenPoint = Camera.main.WorldToScreenPoint(player.transform.position);

        // 将屏幕点转换为Canvas中的本地坐标
        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas.transform as RectTransform, screenPoint, canvas.worldCamera, out Vector2 localPoint);

        // 更新遮罩的位置
        maskRectTransform.localPosition = localPoint;
    }
}

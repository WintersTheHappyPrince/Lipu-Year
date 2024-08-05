using UnityEngine;
using UnityEngine.UI;

public class TransitionEffect : MonoBehaviour
{
    public GameObject player; // 引用玩家对象
    public GameObject circleMask; // 引用CircleMask对象
    public Canvas canvas; // 引用Transition的Canvas对象
    public float transitionSpeed = 2f; // 转场速度
    public float minScale = 0f; // 缩放的最小值
    public float maxScale = 1f; // 缩放的最大值

    private RectTransform maskRectTransform;
    public bool isTransitioning = false;
    private bool isExpanding = false;
    private Vector3 targetScale;

    void Start()
    {
        if (circleMask != null)
        {
            maskRectTransform = circleMask.GetComponent<RectTransform>();
        }

        PlayerRespawn();
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
        targetScale = Vector3.zero; // 目标缩放为0
        UpdateMaskPosition();
    }

    public void PlayerRespawn()
    {
        isTransitioning = true;
        isExpanding = true;
        targetScale = Vector3.one; // 目标缩放为1
        UpdateMaskPosition();
    }

    private void ShrinkMask()
    {
        if (maskRectTransform == null)
        {
            Debug.LogError("maskRectTransform is not assigned.");
            return;
        }

        // 使用 Time.deltaTime 和 transitionSpeed 计算缩放
        maskRectTransform.localScale = Vector3.MoveTowards(maskRectTransform.localScale, targetScale, transitionSpeed * Time.deltaTime);

        if (Vector3.Distance(maskRectTransform.localScale, targetScale) < 0.01f)
        {
            maskRectTransform.localScale = targetScale;
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

        // 使用 Time.deltaTime 和 transitionSpeed 计算缩放
        maskRectTransform.localScale = Vector3.MoveTowards(maskRectTransform.localScale, targetScale, transitionSpeed * Time.deltaTime);

        if (Vector3.Distance(maskRectTransform.localScale, targetScale) < 0.01f)
        {
            maskRectTransform.localScale = targetScale;
            isTransitioning = false;
            // 玩家重生后其他逻辑
        }
    }


    //private void ShrinkMask()
    //{
    //    if (maskRectTransform == null)
    //    {
    //        Debug.LogError("maskRectTransform is not assigned.");
    //        return;
    //    }

    //    float t = Mathf.Clamp01(transitionSpeed * Time.deltaTime);
    //    float smoothT = Mathf.SmoothStep(0f, 1f, t);
    //    maskRectTransform.localScale = Vector3.Lerp(maskRectTransform.localScale, targetScale, smoothT);

    //    if (Vector3.Distance(maskRectTransform.localScale, targetScale) < 0.01f)
    //    {
    //        maskRectTransform.localScale = targetScale;
    //        isTransitioning = false;
    //        // 玩家死亡后其他逻辑
    //    }
    //}

    //private void ExpandMask()
    //{
    //    if (maskRectTransform == null)
    //    {
    //        Debug.LogError("maskRectTransform is not assigned.");
    //        return;
    //    }

    //    float t = Mathf.Clamp01(transitionSpeed * Time.deltaTime);
    //    float smoothT = Mathf.SmoothStep(0f, 1f, t);
    //    maskRectTransform.localScale = Vector3.Lerp(maskRectTransform.localScale, targetScale, smoothT);

    //    if (Vector3.Distance(maskRectTransform.localScale, targetScale) < 0.01f)
    //    {
    //        maskRectTransform.localScale = targetScale;
    //        isTransitioning = false;
    //        // 玩家重生后其他逻辑
    //    }
    //}

    private void UpdateMaskPosition()
    {
        if (player == null || maskRectTransform == null || canvas == null)
        {
            Debug.LogError("player, maskRectTransform, or canvas is not assigned.");
            return;
        }

        // 获取玩家在屏幕上的位置
        Vector3 screenPoint = Camera.main.WorldToScreenPoint(player.transform.position);

        // 更新遮罩的位置
        maskRectTransform.position = screenPoint;
    }
}

using UnityEngine;
using UnityEngine.UI;

public class TransitionEffect : MonoBehaviour
{
    public Image maskImage; // 引用CircleMask的Image组件
    public float transitionSpeed = 2f; // 转场速度

    private bool isTransitioning = false;
    private bool isExpanding = false;

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
    }

    public void PlayerRespawn()
    {
        isTransitioning = true;
        isExpanding = true;
    }

    private void ShrinkMask()
    {
        if (maskImage == null)
        {
            Debug.LogError("maskImage is not assigned.");
            return;
        }
        maskImage.rectTransform.localScale -= Vector3.one * transitionSpeed * Time.deltaTime;
        if (maskImage.rectTransform.localScale.x <= 0.1f)
        {
            maskImage.rectTransform.localScale = Vector3.zero;
            isTransitioning = false;
            // 玩家死亡后其他逻辑
        }
    }

    private void ExpandMask()
    {
        if (maskImage == null)
        {
            Debug.LogError("maskImage is not assigned.");
            return;
        }
        maskImage.rectTransform.localScale += Vector3.one * transitionSpeed * Time.deltaTime;
        if (maskImage.rectTransform.localScale.x >= 1f)
        {
            maskImage.rectTransform.localScale = Vector3.one;
            isTransitioning = false;
            // 玩家重生后其他逻辑
        }
    }
}

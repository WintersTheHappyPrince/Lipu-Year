using UnityEngine;
using UnityEngine.UI;

public class TransitionEffect : MonoBehaviour
{
    public Image maskImage; // ����CircleMask��Image���
    public float transitionSpeed = 2f; // ת���ٶ�

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
            // ��������������߼�
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
            // ��������������߼�
        }
    }
}

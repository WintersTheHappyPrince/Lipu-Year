using UnityEngine;
using UnityEngine.UI;

public class TransitionEffect : MonoBehaviour
{
    public GameObject player; // ������Ҷ���
    public GameObject circleMask; // ����CircleMask����
    public Canvas canvas; // ����Transition��Canvas����
    public float transitionSpeed = 2f; // ת���ٶ�

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
            // ��������������߼�
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
            // ��������������߼�
        }
    }

    private void UpdateMaskPosition()
    {
        if (player == null || maskRectTransform == null || canvas == null)
        {
            Debug.LogError("player, maskRectTransform or canvas is not assigned.");
            return;
        }

        // ��ȡ�������Ļ�ϵ�λ��
        Vector3 screenPoint = Camera.main.WorldToScreenPoint(player.transform.position);

        // ����Ļ��ת��ΪCanvas�еı�������
        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas.transform as RectTransform, screenPoint, canvas.worldCamera, out Vector2 localPoint);

        // �������ֵ�λ��
        maskRectTransform.localPosition = localPoint;
    }
}

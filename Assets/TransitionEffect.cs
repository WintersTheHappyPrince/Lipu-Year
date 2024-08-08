using UnityEngine;
using UnityEngine.UI;

public class TransitionEffect : MonoBehaviour
{
    public GameObject player; // ������Ҷ���
    public GameObject circleMask; // ����CircleMask����
    public Canvas canvas; // ����Transition��Canvas����
    public float transitionSpeed = 2f; // ת���ٶ�
    public float minScale = 0f; // ���ŵ���Сֵ
    public float maxScale = 1f; // ���ŵ����ֵ

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
        targetScale = Vector3.zero; // Ŀ������Ϊ0
        UpdateMaskPosition();
    }

    public void PlayerRespawn()
    {
        isTransitioning = true;
        isExpanding = true;
        targetScale = Vector3.one; // Ŀ������Ϊ1
        UpdateMaskPosition();
    }

    private void ShrinkMask()
    {
        if (maskRectTransform == null)
        {
            Debug.LogError("maskRectTransform is not assigned.");
            return;
        }

        // ʹ�� Time.deltaTime �� transitionSpeed ��������
        maskRectTransform.localScale = Vector3.MoveTowards(maskRectTransform.localScale, targetScale, transitionSpeed * Time.deltaTime);

        if (Vector3.Distance(maskRectTransform.localScale, targetScale) < 0.01f)
        {
            maskRectTransform.localScale = targetScale;
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

        // ʹ�� Time.deltaTime �� transitionSpeed ��������
        maskRectTransform.localScale = Vector3.MoveTowards(maskRectTransform.localScale, targetScale, transitionSpeed * Time.deltaTime);

        if (Vector3.Distance(maskRectTransform.localScale, targetScale) < 0.01f)
        {
            maskRectTransform.localScale = targetScale;
            isTransitioning = false;
            // ��������������߼�
        }
    }

    private void UpdateMaskPosition()
    {
        if (player == null || maskRectTransform == null || canvas == null)
        {
            Debug.LogError("player, maskRectTransform, or canvas is not assigned.");
            return;
        }

        // ��ȡ�������Ļ�ϵ�λ��
        Vector3 screenPoint = Camera.main.WorldToScreenPoint(player.transform.position);

        // �������ֵ�λ��
        maskRectTransform.position = screenPoint;
    }
}

using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    // �����״̬
    public enum CheckpointState
    {
        Default,
        Active
    }

    public CheckpointState state; // ��ǰ״̬

    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Sprite[] sprites;

    //public System.Action CheckpointSave;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        // ��ʼ���������ʾͼ��
        UpdateSprite();
    }

    public void SetState(CheckpointState newState)
    {
        state = newState;
        UpdateSprite();
    }

    private void UpdateSprite()
    {
        // ����״̬����ͼ��
        switch (state)
        {
            case CheckpointState.Default:
               spriteRenderer.sprite = sprites[0]; // Ĭ��״̬ͼ��
                break;
            case CheckpointState.Active:
               spriteRenderer.sprite = sprites[1]; // ����״̬ͼ��
                break;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (state == CheckpointState.Default)
            {
                // �������������ʱ������Ϊ��ǰ�������
                CheckpointManager.instance.SetActiveCheckpoint(this);
                //CheckpointSave?.Invoke();

            }

        }
    }
}

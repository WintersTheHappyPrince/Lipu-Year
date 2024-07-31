using System.Collections;
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

        
    private void UpdateSprite() // ����״̬����ͼ��
    {
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

    private void OnTriggerEnter2D(Collider2D collision)
    {
        PlayerController player = collision.gameObject.GetComponent<PlayerController>();
        if (player != null)
        {
            if (state == CheckpointState.Default)
            {
                StartCoroutine(nameof(CheckpointSave));
            }
        }
    }

    private IEnumerator CheckpointSave()
    {
        yield return new WaitForSeconds(0.5f);
        if(PlayerManager.instance.player.isGrounded && !PlayerManager.instance.player.isDead && !PlayerManager.instance.player.drillingCoroutineRunning)
        {
            CheckpointManager.instance.SetActiveCheckpoint(this);
        }
    }

    //private void OnTriggerEnter2D(Collider2D other)
    //{
    //    if (other.CompareTag("Player"))
    //    {
    //        PlayerController player = other.GetComponent<PlayerController>();
    //        if (player.isGrounded)
    //        {
    //            if (state == CheckpointState.Default)
    //            {
    //                // �������������ʱ������Ϊ��ǰ�������
    //                CheckpointManager.instance.SetActiveCheckpoint(this);
    //            }
    //        }
    //    }
    //}
}

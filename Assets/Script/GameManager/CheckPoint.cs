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
    [SerializeField] private CameraParentTriggerHandler cameraScript;

    private void Start()
    {
        cameraScript = FindObjectOfType<CameraParentTriggerHandler>();
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

    private void OnTriggerStay2D(Collider2D collision)
    {
        PlayerController player = collision.gameObject.GetComponent<PlayerController>();
        if (player != null && !cameraScript.isMoving)
        {
            if ((player.isGrounded && state == CheckpointState.Default && !SaveCoroutine) ||
                (PlayerPrefs.GetInt("IsPlayerInverted") == 1 && !player.isInverted) ||
                (PlayerPrefs.GetInt("IsPlayerInverted") == 0 && player.isInverted))
            {
                StartCoroutine(CheckpointSave());
            }
        }
    }

    private bool SaveCoroutine;
    private IEnumerator CheckpointSave()
    {
        SaveCoroutine = true;
        yield return new WaitForSeconds(0.1f);
        if (PlayerManager.instance.player.isGrounded && !PlayerManager.instance.player.isDead && !PlayerManager.instance.player.dangerOfNails)
        {
            CheckpointManager.instance.SetActiveCheckpoint(this);
            AudioManager.instance.PlaySFX(0);
        }
        SaveCoroutine = false;
    }
}

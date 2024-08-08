using System.Collections;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    // 检查点的状态
    public enum CheckpointState
    {
        Default,
        Active
    }

    public CheckpointState state; // 当前状态

    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Sprite[] sprites;
    [SerializeField] private CameraParentTriggerHandler cameraScript;

    private void Start()
    {
        cameraScript = FindObjectOfType<CameraParentTriggerHandler>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        // 初始化检查点的显示图像
        UpdateSprite();
    }

    public void SetState(CheckpointState newState)
    {
        state = newState;
        UpdateSprite();
    }

        
    private void UpdateSprite() // 根据状态更新图像
    {
        switch (state)
        {
            case CheckpointState.Default:
               spriteRenderer.sprite = sprites[0]; // 默认状态图像
                break;
            case CheckpointState.Active:
               spriteRenderer.sprite = sprites[1]; // 激活状态图像
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

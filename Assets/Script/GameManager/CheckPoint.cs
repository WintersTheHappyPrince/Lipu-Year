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

    private void Start()
    {
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
    //                // 当玩家碰到检查点时，设置为当前激活检查点
    //                CheckpointManager.instance.SetActiveCheckpoint(this);
    //            }
    //        }
    //    }
    //}
}

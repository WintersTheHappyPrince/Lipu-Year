using UnityEngine;

public class MoveState : PlayerState
{
    public MoveState(PlayerController player) : base(player) { }

    public override void Enter()
    {
        player.anim.SetBool("IsMoving", true);
        if (!player.isColorCoroutineRunning && player.sr.color != player.normalColor) player.StartSetDefaultColor(0.2f);
    }

    public override void Update()
    {
        if (!player.isGrounded)
        {
            player.ChangeState(player.airState);
        }
        if (player.isGrounded)
        {
            if (!player.isMoving)
            {
                player.ChangeState(player.idleState);
            }
        }
    }

    public override void Exit()
    {
        // �뿪�ƶ�״̬ʱ���߼�
        player.anim.SetBool("IsMoving", false);
    }
}

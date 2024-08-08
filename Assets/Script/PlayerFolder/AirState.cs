using UnityEngine;

public class AirState : PlayerState
{
    public AirState(PlayerController player) : base(player) { }

    public override void Enter()
    {
        // �����ɫ����״̬���߼�
        player.anim.SetBool("InAir", true);
    }

    public override void Update()
    {
        if (!player.isGrounded) return;
        if (player.isGrounded)
        {
            if (player.isMoving)
            {
            player.ChangeState(player.moveState);
            }
            else
            {
            player.ChangeState(player.idleState);
            }
        }
    }

    public override void Exit()
    {
        // �뿪����״̬ʱ���߼�
        player.anim.SetBool("isGrounded", true);
        player.anim.SetBool("InAir", false);
        player.anim.SetBool("isGrounded", false);
    }
}

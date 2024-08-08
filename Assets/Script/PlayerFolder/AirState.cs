using UnityEngine;

public class AirState : PlayerState
{
    public AirState(PlayerController player) : base(player) { }

    public override void Enter()
    {
        // 处理角色空中状态的逻辑
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
        // 离开空中状态时的逻辑
        player.anim.SetBool("isGrounded", true);
        player.anim.SetBool("InAir", false);
        player.anim.SetBool("isGrounded", false);
    }
}

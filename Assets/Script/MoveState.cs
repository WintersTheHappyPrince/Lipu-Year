using UnityEngine;

public class MoveState : PlayerState
{
    public MoveState(PlayerController player) : base(player) { }

    public override void Enter()
    {
        player.anim.SetBool("IsMoving", true);
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
        // 离开移动状态时的逻辑
        player.anim.SetBool("IsMoving", false);
    }
}

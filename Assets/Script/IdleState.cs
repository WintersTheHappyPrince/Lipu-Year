using UnityEngine;

public class IdleState : PlayerState
{
    public IdleState(PlayerController player) : base(player) { }

    public override void Enter()
    {
        //player.sr.color = player.normalColor;
        // 可以在这里触发静置时的动画
    }

    public override void Update()
    {
        if (player.isMoving)
        {
            player.ChangeState(player.moveState);
        }
        else if (!player.isGrounded)
        {
            player.ChangeState(player.airState);
        }
    }

    public override void Exit()
    {
        // 离开静置状态时的逻辑
    }
}

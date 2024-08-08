using UnityEngine;

public class IdleState : PlayerState
{
    public IdleState(PlayerController player) : base(player) { }

    public override void Enter()
    {
        if (!player.isColorCoroutineRunning && player.sr.color!=player.normalColor) player.StartSetDefaultColor(0.2f);
        //player.UpdateColor();
        // 可以在这里触发静置时的动画
    }

    public override void Update()
    {
        if (!player.isGrounded)
        {
            player.ChangeState(player.airState);
        }
        else if (player.isMoving)
        {
            player.ChangeState(player.moveState);
        }
        
    }

    public override void Exit()
    {
        // 离开静置状态时的逻辑
    }
}

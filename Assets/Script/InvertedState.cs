using UnityEngine;

public class InvertedState : PlayerState
{
    public InvertedState(PlayerController player) : base(player) { }

    public override void Enter()
    {
        player.sr.color = player.invertedColor;
        player.rb.gravityScale = -1;
        // 可以在这里触发颠倒状态的动画
    }

    public override void Update()
    {
        if (player.isGrounded)
        {
            player.ChangeState(player.idleState);
        }
    }

    public override void Exit()
    {
        player.rb.gravityScale = 1;
        // 离开颠倒状态时的逻辑
    }
}

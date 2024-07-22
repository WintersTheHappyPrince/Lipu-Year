using UnityEngine;

public class DrillingState : PlayerState
{
    public DrillingState(PlayerController player) : base(player) { }

    public override void Enter()
    {
        player.sr.color = player.drillColor;
        player.rb.velocity = new Vector2(0, -player.drillSpeed);
        // 可以在这里触发钻地状态的动画
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
        // 离开钻地状态时的逻辑
    }
}

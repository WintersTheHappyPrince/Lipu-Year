using UnityEngine;

public class BouncingState : PlayerState
{
    public BouncingState(PlayerController player) : base(player) { }

    public override void Enter()
    {
        player.sr.color = player.bounceColor;
        player.rb.velocity = new Vector2(player.rb.velocity.x, player.bounceHeight);
        // ���������ﴥ������״̬�Ķ���
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
        // �뿪����״̬ʱ���߼�
    }
}

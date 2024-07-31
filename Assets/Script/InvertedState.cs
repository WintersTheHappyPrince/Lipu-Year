using UnityEngine;

public class InvertedState : PlayerState
{
    public InvertedState(PlayerController player) : base(player) { }

    public override void Enter()
    {
        player.sr.color = player.invertedColor;
        player.rb.gravityScale = -1;
        // ���������ﴥ���ߵ�״̬�Ķ���
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
        // �뿪�ߵ�״̬ʱ���߼�
    }
}

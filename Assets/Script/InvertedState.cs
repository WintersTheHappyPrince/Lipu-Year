using UnityEngine;

public class InvertedState : PlayerState
{
    public InvertedState(PlayerController player) : base(player) { }

    public override void Enter()
    {
        player.sr.color = player.invertedColor;
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
        // �뿪�ߵ�״̬ʱ���߼�
    }
}

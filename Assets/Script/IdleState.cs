using UnityEngine;

public class IdleState : PlayerState
{
    public IdleState(PlayerController player) : base(player) { }

    public override void Enter()
    {
        //player.sr.color = player.normalColor;
        // ���������ﴥ������ʱ�Ķ���
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
        // �뿪����״̬ʱ���߼�
    }
}

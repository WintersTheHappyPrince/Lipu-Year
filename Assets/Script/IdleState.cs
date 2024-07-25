using UnityEngine;

public class IdleState : PlayerState
{
    public IdleState(PlayerController player) : base(player) { }

    public override void Enter()
    {
        if (!player.isColorCoroutineRunning && player.sr.color!=player.normalColor) player.StartSetDefaultColor(0.2f);
        //player.UpdateColor();
        // ���������ﴥ������ʱ�Ķ���
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
        // �뿪����״̬ʱ���߼�
    }
}

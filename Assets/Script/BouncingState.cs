using UnityEngine;

public class BouncingState : PlayerState
{
    public BouncingState(PlayerController player) : base(player) { }

    public override void Enter()
    {
        //player.sr.color = player.bounceColor;
        player.rb.velocity = new Vector2(player.rb.velocity.x, player.bounceHeight);
        // ���������ﴥ������״̬�Ķ���
        player.ChangeState(player.airState);
        player.StartBounceRotate(720);
    }

    public override void Update()
    {
        
    }

    public override void Exit()
    {
        // �뿪����״̬ʱ���߼�
    }
}

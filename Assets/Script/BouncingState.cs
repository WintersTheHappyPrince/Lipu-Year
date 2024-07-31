using UnityEngine;

public class BouncingState : PlayerState
{
    public BouncingState(PlayerController player) : base(player) { }

    public override void Enter()
    {
        //��Ծ��ͷ
        player.jumpTimeCounter = 0;
        player.isJumping = true;
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
        player.anim.transform.rotation = Quaternion.identity;
        // �뿪����״̬ʱ���߼�
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlopeState : PlayerState
{
    public SlopeState(PlayerController player) : base(player) { }

    public override void Enter()
    {
        player.anim.SetBool("Slope", true);
    }

    public override void Update()
    {
        
    }

    public override void Exit()
    {
        player.anim.SetBool("Slope", false);
    }
}

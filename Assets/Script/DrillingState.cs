using System.Collections;
using UnityEngine;
using static PlayerController;

public class DrillingState : PlayerState
{
    public DrillingState(PlayerController player) : base(player) { }

    public override void Enter()
    {
        player.sr.color = player.drillColor;
        Debug.Log("Enter Drilling State");
    }

    public override void Update()
    {
       
    }

    public override void Exit()
    {
        player.fallDistance = 0;
        Debug.Log("Exit Drilling State");
        // 离开钻地状态时的逻辑
    }
}

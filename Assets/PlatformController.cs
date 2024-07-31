using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformController : MonoBehaviour
{
    private PlatformEffector2D pe;
    private PlayerController player;
    void Start()
    {
        pe = GetComponent<PlatformEffector2D>();
        player = PlayerManager.instance.player;
        player.InvertedSystemAction += PlayerInverted;
    }

    private void PlayerInverted()
    {
        if(pe.rotationalOffset == 0)
        {
            pe.rotationalOffset = 180;
        }
        else
        {
            pe.rotationalOffset = 0;
        }
    }
}

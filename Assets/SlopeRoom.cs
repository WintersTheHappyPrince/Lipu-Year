using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using UnityEngine;
using UnityEngine.Windows;

public class SlopeRoom : MonoBehaviour
{
    private Collider2D cd;
    private PlayerController player;

    private void Start()
    {
        cd = GetComponent<Collider2D>();
        player = PlayerManager.instance.player;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("Enter Trigger");
        if(collision.CompareTag("Player"))
        {
            StartCoroutine("SlopeCheckOn");
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        Debug.Log("Exit Trigger");
        if (collision.CompareTag("Player"))
        {
            StopAllCoroutines();
            player.ChangeState(player.idleState);
        }
    }

    private IEnumerator SlopeCheckOn()
    {
        while (true)
        {
            //Debug.Log("Corouttine running");
            yield return null;
            RaycastHit2D[] hits = Physics2D.RaycastAll(player.groundCheck.position, Vector2.down,player.groundCheckRadius);
            
            foreach(RaycastHit2D hit in hits)
            {
                if (hit.collider != null && hit.collider.CompareTag("RightSlope"))
                {
                    if ((!player.isMoving) || (player.isMoving && player.xInput == 1))
                    {
                        Debug.Log("slopeState");
                        player.ChangeState(player.slopeState);

                        if (!player.isMoving)
                        {
                            if (!player.isFacingRight)
                            {
                                Vector3 scale = player.transform.localScale;
                                scale.x *= -1;
                                player.transform.localScale = scale;
                                player.isFacingRight = true;
                            }
                        }
                    }
                    else if (player.isMoving && player.xInput == -1)
                    {
                        player.ChangeState(player.moveState);
                    }
                    else if (player.isJumping)
                    {
                        player.ChangeState(player.airState);
                    }
                }

                else if (hit.collider != null && hit.collider.CompareTag("LeftSlope"))
                {
                    if ((!player.isMoving) || (player.isMoving && player.xInput == -1))
                    {
                        Debug.Log("slopeState");
                        player.ChangeState(player.slopeState);

                        if (!player.isMoving)
                        {
                            if (player.isFacingRight)
                            {
                                Vector3 scale = player.transform.localScale;
                                scale.x *= -1;
                                player.transform.localScale = scale;
                                player.isFacingRight = false;
                            }
                        }
                    }
                    else if (player.isMoving && player.xInput == 1)
                    {
                        player.ChangeState(player.moveState);
                    }
                    else if (player.isJumping)
                    {
                        player.ChangeState(player.airState);
                    }
                }

                else
                {
                    CancelInvoke();
                    Invoke(nameof(ExitSlopeState), 0.5f);
                }
            }
        }
    }

    private void ExitSlopeState()
    {
        if (player.isMoving)
        {
            player.ChangeState(player.moveState);
        }
        if (!player.isMoving)
        {
            player.ChangeState(player.idleState);
        }
        if (!player.isGrounded)
        {
            player.ChangeState(player.airState);
        }
    }
}

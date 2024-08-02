using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trampoline : MonoBehaviour
{
    private Animator anim;
    private Collider2D cd;
    [SerializeField] private float jumpForce;
    private bool active;
    private PlayerController player;

    private void Start()
    {
        player = PlayerManager.instance.player;
        cd = GetComponent<Collider2D>();
        anim = GetComponent<Animator>();
        player.RespawnSystemAction += SetAnim;
    }

    private void SetAnim()
    {
        if (active) //¶¯»­¸´Î»
        {
            transform.Translate(new Vector3(0, 0.15f));
        }
        active = false;
        int i = Random.Range(0, 2);
        if (i == 1)
        {
            int o;
            if (Random.Range(0, 100) > 50) o = 1;
            else o = -1;
            transform.localScale = new Vector3(o,1);
        }
        anim.SetInteger("Idle", i);
        anim.SetBool("Rest", false);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        PlayerController player = collision.gameObject.GetComponent<PlayerController>();
        if (!active)
        {
            if (player != null)
            {
                if (cd.bounds.center.y < player.cd.bounds.min.y)
                {
                    player.StopBounceRotate();
                    player.rb.velocity = new Vector2(0, jumpForce);
                    anim.SetBool("Active", true);
                    active = true;
                    Invoke(nameof(EndAnim), 1);
                }
            }
        }
    }

    private void EndAnim()
    {
        anim.SetBool("Active", false);
        transform.Translate(new Vector3(0, -0.15f)); //¶¯»­Æ¥Åä
        anim.SetBool("Rest", true);
    }
}

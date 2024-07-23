using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trampoline : MonoBehaviour
{
    private Collider2D cd;
    [SerializeField] private float jumpForce;

    private void Start()
    {
        cd=GetComponent<Collider2D>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        PlayerController player = collision.gameObject.GetComponent<PlayerController>();
        if (player != null)
        {
            if (cd.bounds.max.y < player.cd.bounds.min.y)
            {
                Debug.Log("ifÖ´ÐÐ");
                player.rb.velocity = new Vector2(0,jumpForce);  
            }
        }
    }

}

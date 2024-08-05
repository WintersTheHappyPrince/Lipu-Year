using UnityEngine;

public class Nail : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        PlayerController player = collision.gameObject.GetComponent<PlayerController>();
        if (player != null)
        {
            if (((!player.isInverted) && (player.fallDistance > 0.3)) || ((player.isInverted) && (player.rb.velocity.y < -1)) || player.dangerOfNails)
            {
                Debug.Log("Playerfalldis :" + player.fallDistance+ "player.rb.velocity.y : " + player.rb.velocity.y);
                player.killedByNail = true;
                player.Die();
            }
        }
        
    }
}

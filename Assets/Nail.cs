using UnityEngine;

public class Nail : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        PlayerController player = collision.gameObject.GetComponent<PlayerController>();
        if (player != null)
        {
            if (player.fallDistance > 0.3 || player.dangerOfNails)
            {
                player.killedByNail = true;
                player.Die();
            }
        }
        
    }
}

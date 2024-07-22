using Unity.VisualScripting;
using UnityEngine;

public class CameraChildTriggerForwarder : MonoBehaviour
{
    public CameraParentTriggerHandler parentScript;

    void OnTriggerEnter2D(Collider2D other)
    {
        PlayerController playerController = other.GetComponent<PlayerController>();

        if (playerController != null)
            parentScript?.OnChildTriggerEnter(GetComponent<Collider2D>(), other);

    }

    
}

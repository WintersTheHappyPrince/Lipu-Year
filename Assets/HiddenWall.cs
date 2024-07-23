using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class HiddenWall : MonoBehaviour
{
    private Tilemap tm;

    private void Start()
    {
        tm = GetComponent<Tilemap>();
    }

    private void OnTriggerStay2D(Collider2D player)
    {
        if (player.CompareTag("Player"))
        {
            Debug.Log("check");
            tm.color = new Color(1, 1, 1, 0.5f);
        }
        //StopAllCoroutines();
        //StartCoroutine(nameof(Exit));
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!this.isActiveAndEnabled) return;
        StopAllCoroutines();
        StartCoroutine(nameof(Exit));
    }

    private IEnumerator Exit()
    {
        yield return new WaitForSeconds(0.5f);
        tm.color = new Color(1, 1, 1, 1);
    }
}

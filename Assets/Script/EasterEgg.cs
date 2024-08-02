using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EasterEgg : MonoBehaviour
{
    private Collider2D cd;
    private SpriteRenderer sr;

    private void Start()
    {
        sr = GetComponentInChildren<SpriteRenderer>();
        cd = GetComponent<Collider2D>();
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            StartCoroutine(See(collision));
        }
    }

    private IEnumerator See(Collider2D cd)
    {
        Vector3 newScale = sr.transform.localScale;
        
        yield return new WaitForSeconds(0.5f);
        if (cd.transform.position.x > transform.position.x)
        {
            newScale.x = 1;
            sr.transform.localScale = newScale;
        }
        else
        {
            newScale.x = -1;
            sr.transform.localScale = newScale;
        }
        Debug.Log("See");
    }
}

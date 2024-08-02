using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PromptBox : MonoBehaviour
{
    [SerializeField] private TMP_Text textMeshPro;

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (IsMouseOrKeyboardInput())
            {
                textMeshPro.text = "Shift";

                Debug.Log("keymouse");
                if (!ctr)
                    StartCoroutine(MouseOrKey());
            }
            else if (IsControllerInput() && !ctr)
            {
                textMeshPro.text = "LB/RB";
                Debug.Log("joystick");
            }
        }
    }

    private bool IsControllerInput()
    {
        // ºÏ≤‚ ÷±˙ ‰»Î
        return Input.GetAxis("Horizontal") != 0||
               Input.GetAxis("Vertical") != 0 ||
               Input.GetButton("JoyFire1") ||
               Input.GetButton("JoyFire2") ||
               Input.GetButton("JoyFire3") ||
               Input.GetButton("JoyJump");
    }

    private bool IsMouseOrKeyboardInput()
    {
        // ºÏ≤‚º¸≈ÃªÚ Û±Í ‰»Î
        return Input.GetKey(KeyCode.W) ||
               Input.GetKey(KeyCode.A) ||
               Input.GetKey(KeyCode.S) ||
               Input.GetKey(KeyCode.D) ||
               Input.GetAxis("Mouse X") != 0 ||
               Input.GetAxis("Mouse Y") != 0 ||
               Input.GetMouseButton(0) ||
               Input.GetMouseButton(1) ||
               Input.GetMouseButton(2);
    }

    private bool ctr;
    private IEnumerator MouseOrKey()
    {
        ctr = true;
        yield return new WaitForSeconds(0.5f);
        ctr = false;
    }
}

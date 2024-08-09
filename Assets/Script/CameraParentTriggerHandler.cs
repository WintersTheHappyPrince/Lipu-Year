using System.Collections;
using UnityEngine;

public class CameraParentTriggerHandler : MonoBehaviour
{
    private GameObject blockPlayer;
    public float moveSpeed = 100f; // �ƶ��ٶȣ�Ӱ��ƽ���ȣ�
    private Vector3 startPosition;
    private Vector3 targetPosition;
    private float startTime;
    public bool isMoving = false;

    private GameObject gridLine;
    private bool isActive;
    private Coroutine blockPlayerCoroutine;

    //�ֻ���
    private Vector2 startTouchPosition;
    private bool swipeDetected = false;
    private float minSwipeDistance;

    void Start()
    {
        blockPlayer = GameObject.FindWithTag("PlayerBlocker");
        if (blockPlayer != null)
        {
            blockPlayer.SetActive(false);
        }

        gridLine = FindObjectOfType<GridDrawer>().gameObject;

        // ��ȡ�����Ӷ����Collider
        Collider2D[] childColliders = GetComponentsInChildren<Collider2D>();
        foreach (var childCollider in childColliders)
        {
            // Ϊÿ���Ӷ�����Ӵ����¼�ת����
            CameraChildTriggerForwarder forwarder = childCollider.gameObject.AddComponent<CameraChildTriggerForwarder>();
            forwarder.parentScript = this;
        }


        // �ֻ��˼�����Ļ�߶ȵ�һ����Ϊ��С��������
        minSwipeDistance = Screen.height / 2f;
    }

    void Update()
    {
        if (isMoving)
        {
            float journeyLength = Vector3.Distance(startPosition, targetPosition);
            float distanceCovered = (Time.time - startTime) * moveSpeed;
            float fractionOfJourney = distanceCovered / journeyLength;

            // ƽ�����ɵ�Ŀ��λ��
            transform.position = Vector3.Lerp(startPosition, targetPosition, fractionOfJourney);

            // ����Ƿ񵽴�Ŀ��λ��
            if (fractionOfJourney >= 1f)
            {
                transform.position = targetPosition; // ȷ������λ��׼ȷ
                isMoving = false; // ֹͣ�ƶ�
            }
        }

        if (Input.GetKeyDown(KeyCode.LeftShift) ||
            Input.GetKeyDown(KeyCode.RightShift) ||
            Input.GetKeyDown(KeyCode.JoystickButton4) ||
            Input.GetKeyDown(KeyCode.JoystickButton5))
            SetGridLineIsActive();

        //�ֻ��������ߴ���
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            // ��ⴥ����ʼ
            if (touch.phase == TouchPhase.Began)
            {
                startTouchPosition = touch.position;
                swipeDetected = false;

                // ȷ����������Ļ����
                if (startTouchPosition.x <= Screen.width / 2)
                {
                    swipeDetected = true;
                }
            }

            // ��ⴥ���ƶ�
            if (touch.phase == TouchPhase.Moved && swipeDetected)
            {
                Vector2 currentTouchPosition = touch.position;
                float verticalMove = currentTouchPosition.y - startTouchPosition.y;
                float horizontalMove = currentTouchPosition.x - startTouchPosition.x;

                // �жϻ����������»������;����Ƿ񳬹���С��������
                if (Mathf.Abs(verticalMove) > Mathf.Abs(horizontalMove) && Mathf.Abs(verticalMove) >= minSwipeDistance)
                {
                    if (verticalMove > 0)
                    {
                        Debug.Log("Swipe Up Detected on the Left Half of the Screen");
                        SetGridLineIsActive();
                    }
                    else if (verticalMove < 0)
                    {
                        Debug.Log("Swipe Down Detected on the Left Half of the Screen");
                        SetGridLineIsActive();
                    }

                    // �����ѱ����������ظ����
                    swipeDetected = false;
                }
            }

            // �������������ü��
            if (touch.phase == TouchPhase.Ended)
            {
                swipeDetected = false;
            }
        }
    }

    private void SetGridLineIsActive()
    {
        isActive = !isActive;
        if (gridLine != null)
        {
            gridLine.SetActive(isActive);
        }
    }

    // ����ƽ���ƶ�
    private void StartMove(Vector3 offset)
    {
        startPosition = transform.position;
        targetPosition = startPosition + offset;
        startTime = Time.time;
        isMoving = true;
    }

    // ������������Ӷ��󴥷�ʱ������
    public void OnChildTriggerEnter(Collider2D childCollider, Collider2D other)
    {
        if (blockPlayer != null)
        {
            blockPlayer.SetActive(true);
            if (blockPlayerCoroutine != null)
            {
                StopCoroutine(blockPlayerCoroutine);
            }
            blockPlayerCoroutine = StartCoroutine(blockPlayerSetFalse(blockPlayer));
        }

        //���λ��
        switch (childCollider.name)
        {
            case "RightCameraColliderBox":
                StartMove(new Vector3(30, 0));
                blockPlayer.transform.position += new Vector3(30, 0);
                break;
            case "LeftCameraColliderBox":
                StartMove(new Vector3(-30, 0));
                blockPlayer.transform.position += new Vector3(-30, 0);
                break;
            case "UpCameraColliderBox":
                StartMove(new Vector3(0, 16));
                blockPlayer.transform.position += new Vector3(0, 16);
                break;
            case "DownCameraColliderBox":
                StartMove(new Vector3(0, -16));
                blockPlayer.transform.position += new Vector3(0, -16);
                break;
        }
    }

    IEnumerator blockPlayerSetFalse(GameObject blockPlayer)
    {
        yield return new WaitForSeconds(1);
        blockPlayer.SetActive(false);
    }
}

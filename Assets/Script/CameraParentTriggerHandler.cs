using System.Collections;
using UnityEngine;

public class CameraParentTriggerHandler : MonoBehaviour
{
    private GameObject blockPlayer;
    public float moveSpeed = 100f; // 移动速度（影响平滑度）
    private Vector3 startPosition;
    private Vector3 targetPosition;
    private float startTime;
    public bool isMoving = false;

    private GameObject gridLine;
    private bool isActive;
    private Coroutine blockPlayerCoroutine;

    //手机端
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

        // 获取所有子对象的Collider
        Collider2D[] childColliders = GetComponentsInChildren<Collider2D>();
        foreach (var childCollider in childColliders)
        {
            // 为每个子对象添加触发事件转发器
            CameraChildTriggerForwarder forwarder = childCollider.gameObject.AddComponent<CameraChildTriggerForwarder>();
            forwarder.parentScript = this;
        }


        // 手机端计算屏幕高度的一半作为最小滑动距离
        minSwipeDistance = Screen.height / 2f;
    }

    void Update()
    {
        if (isMoving)
        {
            float journeyLength = Vector3.Distance(startPosition, targetPosition);
            float distanceCovered = (Time.time - startTime) * moveSpeed;
            float fractionOfJourney = distanceCovered / journeyLength;

            // 平滑过渡到目标位置
            transform.position = Vector3.Lerp(startPosition, targetPosition, fractionOfJourney);

            // 检查是否到达目标位置
            if (fractionOfJourney >= 1f)
            {
                transform.position = targetPosition; // 确保最终位置准确
                isMoving = false; // 停止移动
            }
        }

        if (Input.GetKeyDown(KeyCode.LeftShift) ||
            Input.GetKeyDown(KeyCode.RightShift) ||
            Input.GetKeyDown(KeyCode.JoystickButton4) ||
            Input.GetKeyDown(KeyCode.JoystickButton5))
            SetGridLineIsActive();

        //手机端网格线处理
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            // 检测触摸开始
            if (touch.phase == TouchPhase.Began)
            {
                startTouchPosition = touch.position;
                swipeDetected = false;

                // 确保触摸在屏幕左半边
                if (startTouchPosition.x <= Screen.width / 2)
                {
                    swipeDetected = true;
                }
            }

            // 检测触摸移动
            if (touch.phase == TouchPhase.Moved && swipeDetected)
            {
                Vector2 currentTouchPosition = touch.position;
                float verticalMove = currentTouchPosition.y - startTouchPosition.y;
                float horizontalMove = currentTouchPosition.x - startTouchPosition.x;

                // 判断滑动方向（上下滑动）和距离是否超过最小滑动距离
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

                    // 滑动已被处理，不再重复检测
                    swipeDetected = false;
                }
            }

            // 触摸结束后重置检测
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

    // 启动平滑移动
    private void StartMove(Vector3 offset)
    {
        startPosition = transform.position;
        targetPosition = startPosition + offset;
        startTime = Time.time;
        isMoving = true;
    }

    // 这个方法将在子对象触发时被调用
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

        //相机位移
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

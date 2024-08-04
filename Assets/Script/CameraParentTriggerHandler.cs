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

    void Start()
    {
        blockPlayer = GameObject.FindWithTag("PlayerBlocker");
        if (blockPlayer != null)
        {
            blockPlayer.SetActive(false);
        }

        gridLine = GetComponentInChildren<GridDrawer>()?.gameObject;

        // 获取所有子对象的Collider
        Collider2D[] childColliders = GetComponentsInChildren<Collider2D>();
        foreach (var childCollider in childColliders)
        {
            // 为每个子对象添加触发事件转发器
            CameraChildTriggerForwarder forwarder = childCollider.gameObject.AddComponent<CameraChildTriggerForwarder>();
            forwarder.parentScript = this;
        }
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
            Input.GetKeyDown(KeyCode.JoystickButton5) )
        {
            isActive = !isActive;
            if (gridLine != null)
            {
                gridLine.SetActive(isActive);
            }
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

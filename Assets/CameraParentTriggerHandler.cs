using System.Collections;
using UnityEngine;

public class CameraParentTriggerHandler : MonoBehaviour
{
    private GameObject blockPlayer;

    public float moveSpeed = 100f; // 移动速度（影响平滑度）
    private Vector3 startPosition;
    private Vector3 targetPosition;
    private float startTime;
    private bool isMoving = false;

    void Start()
    {
        blockPlayer = GameObject.Find("BlockPlayer");
        blockPlayer.SetActive(false);
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
        // 在这里处理触发事件
        blockPlayer.SetActive(true);
        StopCoroutine(blockPlayerSetFalse(blockPlayer));
        StartCoroutine(blockPlayerSetFalse(blockPlayer));

        //相机位移
        //Debug.Log("子对象 " + childCollider.name + " 触发了碰撞: " + other.name);
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
                //blockPlayer.transform.position += new Vector3(0, 16);
                break;
            case "DownCameraColliderBox":
                StartMove(new Vector3(0, -16));
                //blockPlayer.transform.position += new Vector3(0,-16);
                break;
        }
    }

    IEnumerator blockPlayerSetFalse(GameObject blockPlayer)
    {
        yield return new WaitForSeconds(1);
        blockPlayer.SetActive(false); 
    }    
}


using System.Collections;
using UnityEngine;

public class CameraParentTriggerHandler : MonoBehaviour
{
    private GameObject blockPlayer;

    public float moveSpeed = 100f; // �ƶ��ٶȣ�Ӱ��ƽ���ȣ�
    private Vector3 startPosition;
    private Vector3 targetPosition;
    private float startTime;
    private bool isMoving = false;

    void Start()
    {
        blockPlayer = GameObject.Find("BlockPlayer");
        blockPlayer.SetActive(false);
        // ��ȡ�����Ӷ����Collider
        Collider2D[] childColliders = GetComponentsInChildren<Collider2D>();
        foreach (var childCollider in childColliders)
        {
            // Ϊÿ���Ӷ�����Ӵ����¼�ת����
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

            // ƽ�����ɵ�Ŀ��λ��
            transform.position = Vector3.Lerp(startPosition, targetPosition, fractionOfJourney);

            // ����Ƿ񵽴�Ŀ��λ��
            if (fractionOfJourney >= 1f)
            {
                transform.position = targetPosition; // ȷ������λ��׼ȷ
                isMoving = false; // ֹͣ�ƶ�
            }
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
        // �����ﴦ�����¼�
        blockPlayer.SetActive(true);
        StopCoroutine(blockPlayerSetFalse(blockPlayer));
        StartCoroutine(blockPlayerSetFalse(blockPlayer));

        //���λ��
        //Debug.Log("�Ӷ��� " + childCollider.name + " ��������ײ: " + other.name);
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


using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public static PlayerManager instance;

    public PlayerController player;

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // ���ֵ��������ڳ����л�ʱ��������
        }

        if (player == null)
        {
            player = FindObjectOfType<PlayerController>();
            if (player == null)
            {
                Debug.LogError("δ�ҵ�PlayerController�����");
            }
        }

    }
}

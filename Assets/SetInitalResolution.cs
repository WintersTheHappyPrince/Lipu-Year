using UnityEngine;

public class SetInitialResolution : MonoBehaviour
{
    public SetInitialResolution instance;

    public int targetWidth = 1280;
    public int targetHeight = 682;
    public bool isFullScreen = false; // ������Ҫ�����Ƿ�ȫ��
    public float targetAspectRatio = 30f / 16f; // Ŀ���߱�

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        // ���ô��ڷֱ���
        Screen.SetResolution(targetWidth, targetHeight, isFullScreen);
    }

    void Update()
    {
        // ÿ֡��鲢�������ڴ�С
        SetAspectRatio();
    }

    void SetAspectRatio()
    {
        float windowWidth = Screen.width;
        float windowHeight = Screen.height;

        float currentAspectRatio = windowWidth / windowHeight;

        if (Mathf.Abs(currentAspectRatio - targetAspectRatio) > Mathf.Epsilon)
        {
            if (currentAspectRatio > targetAspectRatio)
            {
                // ���ڹ����������
                float newWidth = windowHeight * targetAspectRatio;
                Screen.SetResolution((int)newWidth, Screen.height, isFullScreen);
            }
            else
            {
                // ���ڹ��ߣ������߶�
                float newHeight = windowWidth / targetAspectRatio;
                Screen.SetResolution(Screen.width, (int)newHeight, isFullScreen);
            }
        }
    }
}

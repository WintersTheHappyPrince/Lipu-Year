using UnityEngine;

public class SetInitialResolution : MonoBehaviour
{
    public int targetWidth = 1920;
    public int targetHeight = 1024;
    public bool isFullScreen = false; // 根据需要设置是否全屏
    public float targetAspectRatio = 30f / 16f; // 目标宽高比

    void Start()
    {
        // 设置窗口分辨率
        Screen.SetResolution(targetWidth, targetHeight, isFullScreen);
    }

    void Update()
    {
        // 每帧检查并调整窗口大小
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
                // 窗口过宽，调整宽度
                float newWidth = windowHeight * targetAspectRatio;
                Screen.SetResolution((int)newWidth, Screen.height, isFullScreen);
            }
            else
            {
                // 窗口过高，调整高度
                float newHeight = windowWidth / targetAspectRatio;
                Screen.SetResolution(Screen.width, (int)newHeight, isFullScreen);
            }
        }
    }
}

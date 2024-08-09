using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameClearScript : MonoBehaviour
{
    public static GameClearScript instance;
    public bool isGameCleared;

    private void Awake()
    {
        // 确保这个实例是唯一的
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public bool canReturn;
    [SerializeField]private GameObject returnToMainMenu;

    private void Start()
    {
        returnToMainMenu=GameObject.Find("ReturnToMainMenu");
    }

    private string MenuSceneName = "MainMenu";

    void Update()
    {
        if (!canReturn && returnToMainMenu!=null)
        {
            if (returnToMainMenu.GetComponent<CanvasGroup>().alpha == 1)
                canReturn = true;
        }
        
        if (canReturn)
        {
            // 判断是否有任意键被按下（用于PC）
            bool anyKeyPressed = Input.anyKeyDown;

            // 判断是否有鼠标点击或触摸（用于PC和移动设备）
            bool mouseOrTouchInput = Input.GetMouseButtonDown(0) || Input.touchCount > 0;

            // 如果任意键、鼠标点击或触摸事件触发
            if (anyKeyPressed || mouseOrTouchInput)
            {
                isGameCleared = true;
                canReturn = false;
                SceneManager.LoadScene(MenuSceneName);
            }
        }
    }

    
}

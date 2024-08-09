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
        // ȷ�����ʵ����Ψһ��
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
            // �ж��Ƿ�������������£�����PC��
            bool anyKeyPressed = Input.anyKeyDown;

            // �ж��Ƿ������������������PC���ƶ��豸��
            bool mouseOrTouchInput = Input.GetMouseButtonDown(0) || Input.touchCount > 0;

            // ��������������������¼�����
            if (anyKeyPressed || mouseOrTouchInput)
            {
                isGameCleared = true;
                canReturn = false;
                SceneManager.LoadScene(MenuSceneName);
            }
        }
    }

    
}

using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UI_MainMenu : MonoBehaviour
{
    [SerializeField] private string mainSceneName = "MainScene";
    [SerializeField] private GameObject continueButton;

    private void Start()
    {
        //�˶�if else���Ƽ�����Ϸ�浵
        if (GoalManager.instance.HasSavedData() == false)
        {
            continueButton.GetComponent<Button>().enabled = false;
            continueButton.GetComponent<CanvasGroup>().alpha = 0.25f;
        }
        else
        {
            int goalsCollected = GoalManager.instance.GetCollectedGoals();
            Debug.Log(goalsCollected);
            continueButton.GetComponentInChildren<TextMeshProUGUI>().text = "������Ϸ" + "\n" + $"{goalsCollected}" + "/29";
        }
    }

    public void ContinueGame()
    {
        AudioManager.instance.PlaySFX(5);
        SceneManager.LoadScene(mainSceneName);
    }

    [SerializeField] GameObject newGameButton;
    public void NewGame()
    {
        //�˶�if else��������Ϸ���Ǵ浵
        if (GoalManager.instance.HasSavedData() == false)
        {
            AudioManager.instance.PlaySFX(5);
            PlayerPrefs.DeleteAll();
            SceneManager.LoadScene(mainSceneName);
        }
        else if(GoalManager.instance.HasSavedData() == true)
        {
            AudioManager.instance.PlaySFX(5);
            newGameButton.GetComponentInChildren<TextMeshProUGUI>().text = "����Ϸ?";
            newGameButton.GetComponent<Button>().onClick.AddListener(ConfirmNewGame);
        }
    }

    private void ConfirmNewGame()
    {
        AudioManager.instance.PlaySFX(5);
        PlayerPrefs.DeleteAll();
        SceneManager.LoadScene(mainSceneName);
    }

    public void ExitGame()
    {
        AudioManager.instance.PlaySFX(5);
        Application.Quit();
    }
}

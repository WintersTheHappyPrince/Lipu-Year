using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UI_MainMenu : MonoBehaviour
{
    [SerializeField] private string mainSceneName = "MainScene";
    [SerializeField] private GameObject newGameButton;
    [SerializeField] private GameObject continueButton;

    [SerializeField]private GameObject gameClearScript;

    private void Start()
    {
        gameClearScript = FindObjectOfType<GameClearScript>()?.gameObject;

        //如果已经通关游戏
        if(gameClearScript != null)
        {
            if (gameClearScript.GetComponent<GameClearScript>().isGameCleared)
            {
                newGameButton.GetComponentInChildren<TextMeshProUGUI>().color = Color.red;

                continueButton.GetComponentInChildren<TextMeshProUGUI>().color = Color.blue;
                continueButton.GetComponentInChildren<TextMeshProUGUI>().text = "感谢游玩";
                continueButton.GetComponent<Button>().enabled = false;
                return;
            }
        }

        //此段if else控制有存档情况下改变继续游戏按钮显示
        if (GoalManager.instance.HasSavedData() == false)
        {
            continueButton.GetComponent<Button>().enabled = false;
            continueButton.GetComponent<CanvasGroup>().alpha = 0.25f;
        }
        else if (GoalManager.instance.HasSavedData() == true)
        {
            int goalsCollected = GoalManager.instance.GetCollectedGoals();
            continueButton.GetComponentInChildren<TextMeshProUGUI>().text = "继续游戏" + "\n" + $"{goalsCollected}" + "/29";
        }
    }

    public void ContinueGame()
    {
        AudioManager.instance.PlaySFX(5);
        SceneManager.LoadScene(mainSceneName);
    }

    
    public void NewGame()
    {
        //此段if else控制新游戏覆盖存档
        if (GoalManager.instance.HasSavedData() == false)
        {
            AudioManager.instance.PlaySFX(5);
            PlayerPrefs.DeleteAll();
            SceneManager.LoadScene(mainSceneName);
        }
        else if (GoalManager.instance.HasSavedData() == true)
        {
            AudioManager.instance.PlaySFX(5);
            newGameButton.GetComponentInChildren<TextMeshProUGUI>().text = "新游戏?";
            newGameButton.GetComponent<Button>().onClick.AddListener(ConfirmNewGame);
        }
    }

    private void ConfirmNewGame()
    {
        if (gameClearScript != null)
            Destroy(gameClearScript);
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

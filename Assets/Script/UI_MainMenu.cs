using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UI_MainMenu : MonoBehaviour
{
    [SerializeField] private string sceneName = "MainScene";
    [SerializeField] private GameObject continueButton;

    private void Start()
    {
        if (GoalManager.instance.HasSavedData() == false)
        {
            continueButton.GetComponent<Button>().enabled = false;
            continueButton.GetComponent<CanvasGroup>().alpha = 0.25f;
        }
        else
        {
            int goalsCollected = GoalManager.instance.GetCollectedGoals();
            Debug.Log(goalsCollected);
            continueButton.GetComponentInChildren<TextMeshProUGUI>().text = "¼ÌÐøÓÎÏ·" + "\n" + $"{goalsCollected}" + "/29";
        }
    }

    public void ContinueGame()
    {
        SceneManager.LoadScene(sceneName);
    }

    public void NewGame()
    {
        SceneManager.LoadScene(sceneName);
        PlayerPrefs.DeleteAll();
    }

    public void ExitGame()
    { 
        Application.Quit();
    }
}

using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ButtonUIManager : MonoBehaviour
{
    [SerializeField] public GameObject PauseMenuPanel;
    public void Pause()
    {
        PauseMenuPanel.SetActive(true);
        Time.timeScale = 0f;
    }
    public void Resume()
    {
        PauseMenuPanel.SetActive(false);
        Time.timeScale = 1f;
    }
    public void Restart()
    {
        Time.timeScale = 1f;
        //SceneManager.LoadScene("SampleScene");
        MessageSystem.TriggerEvent(MessageKey.SceneManager.ChangeScene, SceneName.GameplayScene);
    }
    public void BackToHome()
    {
        Time.timeScale = 1f;
        //SceneManager.LoadScene("StartScene");
        MessageSystem.TriggerEvent(MessageKey.SceneManager.ChangeScene, SceneName.StartScene);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public enum SceneName
{
    None,
    StartScene,
    GameplayScene,
    ResultScene
}
public class CustomSceneManager : MonoBehaviour
{
    [SerializeField] private SceneName currentScene;
    public static CustomSceneManager Instance;
    public SceneName CurrentScene => currentScene;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else if (Instance != this)
        {
            Destroy(this.gameObject);
        }
    }

    private void OnEnable()
    {
        MessageSystem.StartListening<SceneName>(MessageKey.SceneManager.ChangeScene, ChangeScene);
    }

    private void ChangeScene(SceneName sceneName)
    {
        StartCoroutine(LoadSceneAsync(sceneName));
    }

    private IEnumerator LoadSceneAsync(SceneName sceneName)
    {
        currentScene = sceneName;     
        var progress = SceneManager.LoadSceneAsync(sceneName.ToString(), LoadSceneMode.Single);

        yield return progress;

    }

    private void OnDisable()
    {
        MessageSystem.StopListening<SceneName>(MessageKey.SceneManager.ChangeScene, ChangeScene);
    }
}

using UnityEngine;
public enum GameMode
{
    None,
    Multiplayer,
    AI
}
public class GameManager : Singleton<GameManager>
{
    [Header("Game Mode")]
    [SerializeField]
    private GameMode currentGameMode;

    public GameMode CurrentGameMode { get => currentGameMode; set => currentGameMode = value; }

    public void InitializeGameMode()
    {
        switch (currentGameMode)
        {
            case GameMode.Multiplayer:
                SetupMultiplayerMode();
                break;
            case GameMode.AI:
                SetupAIMode();
                break;
            default:
                Debug.LogError("Unknown game mode selected!");
                break;
        }
    }


    private void SetupMultiplayerMode()
    {

    }

    private void SetupAIMode()
    {

    }


    public void ChangeGameMode(GameMode newMode)
    {
        currentGameMode = newMode;
        InitializeGameMode();
    }
}

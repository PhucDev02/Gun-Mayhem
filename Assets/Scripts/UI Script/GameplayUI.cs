using TMPro;
using UnityEngine;

public class GameplayUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI player1Lives, player2Lives;

    private void Awake()
    {
        MessageSystem.StartListening<EPlayer, int>(MessageKey.UI.UpdatePlayerLives, UpdatePlayerLives);
    }

    private void OnDestroy()
    {
        MessageSystem.StopListening<EPlayer, int>(MessageKey.UI.UpdatePlayerLives, UpdatePlayerLives);
    }

    private void UpdatePlayerLives(EPlayer player, int lives)
    {
        if(player == EPlayer.BluePlayer)
        {
            player1Lives.text = lives.ToString();
        }
        else
        {
            player2Lives.text = lives.ToString();
        }
    }
}

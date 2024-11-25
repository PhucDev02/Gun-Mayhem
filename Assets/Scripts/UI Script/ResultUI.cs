using Multiplayer.Manager;
using UnityEngine;
using UnityEngine.UI;

public class ResultUI : MonoBehaviour
{
    [SerializeField] private Sprite blueWinSpr, redWinSpr;
    [SerializeField] private Image winImg;

    void Start()
    {
        LobbyManager.Instance.DeleteLobby();
        winImg.sprite = (GameController.winner == EPlayer.BluePlayer)
            ? blueWinSpr : redWinSpr;
    }
}

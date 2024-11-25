using Cysharp.Threading.Tasks;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

public class GameplayUI : MonoBehaviour
{
    [SerializeField] PlayerInfoView[] infoView;
    private async void Start()
    {
        await UniTask.WaitUntil(() => GameController.Instance != null && GameController.Instance.GetTotalPlayer == 2);
        for (int i = 0; i < infoView.Length; i++) {
            PlayerInfoView player = infoView[i];
            Debug.LogError("Player type " + i + " " + GameController.Instance.players[i].type);
            player.Setup(GameController.Instance.players[i].type);
        }
    }

}

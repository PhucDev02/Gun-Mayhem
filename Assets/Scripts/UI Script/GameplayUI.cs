using TMPro;
using UnityEngine;

public class GameplayUI : MonoBehaviour
{
    [SerializeField] PlayerInfoView[] infoView;
    private void Start()
    {
        for (int i = 0; i < infoView.Length; i++) {
            PlayerInfoView player = infoView[i];
            player.Setup(GameController.Instance.players[i].type);
        }
    }

}

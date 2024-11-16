using Google.Protobuf.Collections;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerInfoView : MonoBehaviour
{
    EPlayer type;
    [SerializeField] TextMeshProUGUI playerName;
    [SerializeField] TextMeshProUGUI lives;
    [SerializeField] Image avatar;
    [Header("Sprite")]
    [SerializeField] Sprite redPlayer;
    [SerializeField] Sprite bluePlayer;
    [SerializeField] Sprite AIPlayer;

    public void Setup(EPlayer player)
    {
        playerName.SetText(type.ToString());
        type = player;
        switch (type)
        {
            case EPlayer.BluePlayer:
                avatar.sprite = bluePlayer;
                break;
            case EPlayer.RedPlayer:
                avatar.sprite = redPlayer;
                break;
            case EPlayer.AI:
                avatar.sprite = AIPlayer;
                break;
        }
    }
    private void OnEnable()
    {
        MessageSystem.StartListening<EPlayer, int>(MessageKey.UI.UpdatePlayerLives, UpdatePlayerLives);
    }

    private void UpdatePlayerLives(EPlayer player, int arg2)
    {
        if (player == type)
        {
            lives.SetText(arg2.ToString());
        }
    }

    private void OnDisable()
    {
        MessageSystem.StopListening<EPlayer, int>(MessageKey.UI.UpdatePlayerLives, UpdatePlayerLives);

    }
}

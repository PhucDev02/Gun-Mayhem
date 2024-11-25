using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LobbyPlayer : MonoBehaviour
{
    public TextMeshProUGUI _playerName;
    private LobbyPlayerData _data;
    public Image status;

    public void SetData(LobbyPlayerData data)
    {
        _data = data;
        _playerName.text = _data.GameTag;
        SetReady(_data.IsReady);
        gameObject.SetActive(true);
    }

    public void SetReady(bool ready)
    {
        if (ready) status.color = Color.green;
        else status.color = Color.white;
    }
}

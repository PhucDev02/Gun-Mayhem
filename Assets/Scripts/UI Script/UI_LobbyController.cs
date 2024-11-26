using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UI_LobbyController : MonoBehaviour
{
    [SerializeField] private Button _hostBtn;
    [SerializeField] private Button _joinBtn;

    [SerializeField] private TextMeshProUGUI inputFieldLobby;
    [SerializeField] private GameObject lobbyRoomUI;

    private void OnEnable()
    {
        _hostBtn.onClick.AddListener(OnHostClicked);
        _joinBtn.onClick.AddListener(OnJoinClicked);
    }

    private void OnDisable()
    {
        _hostBtn.onClick.RemoveListener(OnHostClicked);
        _joinBtn.onClick.RemoveListener(OnJoinClicked);
    }

    private async void OnHostClicked()
    {
        bool succeeded =  await GameLobbyManager.Instance.CreateLobby();
        if(succeeded)
        {
            //Debug.Log("Entered Lobby");
            //lobbyCode.text = "Lobby Code: " + GameLobbyManager.Instance.GetLobbyCode();
            lobbyRoomUI.SetActive(true);
        }
    }

    private async void OnJoinClicked()
    {
        string code = inputFieldLobby.text;
        code = code.Substring(0, code.Length - 1);
        bool succeed = await GameLobbyManager.Instance.JoinLobby(code);
        if (succeed)
        {
            Debug.Log("Join Lobby Succeed");
            lobbyRoomUI.SetActive(true);
        }
        Debug.Log(code);
    }
}

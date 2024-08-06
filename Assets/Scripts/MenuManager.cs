using Photon.Pun;
using UnityEngine.UI;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class MenuManager : MonoBehaviourPunCallbacks
{
    [SerializeField] private TMP_InputField nickNameInput;
    private void Start()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        PhotonNetwork.AutomaticallySyncScene = true;
        PhotonNetwork.GameVersion = "1";
        PhotonNetwork.ConnectUsingSettings();
        nickNameInput.text = PlayerPrefs.GetString("NickName");
        PhotonNetwork.NickName = nickNameInput.text;
        nickNameInput.onValueChanged.AddListener(UpdateInputField);
    }

    private void UpdateInputField(string data)
    {
        Debug.Log(nickNameInput.text);
        PhotonNetwork.NickName = nickNameInput.text;
        PlayerPrefs.SetString("NickName", nickNameInput.text);
    }
    public override void OnConnectedToMaster()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
        PhotonNetwork.GameVersion = "1.0";
        PhotonNetwork.JoinLobby();
    }
    public void QuickMatch()
    {
        PhotonNetwork.JoinRandomRoom();
    }

    public override void OnJoinedRoom()
    {
        PhotonNetwork.LoadLevel(1);
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        PhotonNetwork.CreateRoom(null, new Photon.Realtime.RoomOptions { MaxPlayers = 4 });
    }

    public void Quit()
    {
        Application.Quit();
    }
}

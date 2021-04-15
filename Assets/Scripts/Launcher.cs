using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;
using Photon.Realtime;

public class Launcher : MonoBehaviourPunCallbacks
{
    public static Launcher instance;

    [SerializeField] TMP_InputField roomNameInputField;
    [SerializeField] TMP_Text roomNameText;
    [SerializeField] TMP_Text errorText;
    [SerializeField] Transform roomListContent;
    [SerializeField] GameObject roomListItemPrefab;
    [SerializeField] Transform playerListContent;
    [SerializeField] GameObject playerListItemPrefab;
    [SerializeField] GameObject startGameButton;


    void Awake() {
        instance = this;
    }

    void Start()
    {
        Debug.Log("Connecting to master...");
        PhotonNetwork.ConnectUsingSettings();
        MenuManager.instance.OpenMenu("loading");
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("Connected to master");
        Debug.Log("Joining lobby...");
        PhotonNetwork.JoinLobby();
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    public override void OnJoinedLobby()
    {
        Debug.Log("Joined lobby");
        MenuManager.instance.OpenMenu("title");
        PhotonNetwork.NickName = $"Player { Random.Range(0,1000) }";
    }

    public void CreateRoom() {
        PhotonNetwork.CreateRoom(roomNameInputField.text);
        MenuManager.instance.OpenMenu("loading");
        Debug.Log("Creating room...");
    }

    public override void OnJoinedRoom() {
        Debug.Log("Joined room");
        roomNameText.text = PhotonNetwork.CurrentRoom.Name;
        MenuManager.instance.OpenMenu("room");

        foreach (Transform child in playerListContent)
            Destroy(child.gameObject);

        foreach (Player player in PhotonNetwork.PlayerList)
            Instantiate(playerListItemPrefab, playerListContent).GetComponent<PlayerListItem>().Setup(player);
    
        startGameButton.SetActive(PhotonNetwork.IsMasterClient);
    }

    public override void OnMasterClientSwitched(Player newMasterClient) {
        startGameButton.SetActive(PhotonNetwork.IsMasterClient);
    }

    public override void OnCreateRoomFailed(short returnCode, string message) {
        Debug.Log("Failed to create room");
        errorText.text = $"Room creation failed: {message}";
        MenuManager.instance.OpenMenu("error");
    }

    public void StartGame() {
        PhotonNetwork.LoadLevel(1);
    }

    public void LeaveRoom() {
        Debug.Log("Leaving room...");
        PhotonNetwork.LeaveRoom();
        MenuManager.instance.OpenMenu("loading");
    }

    override public void OnLeftRoom() {
        Debug.Log("Left room...");
        MenuManager.instance.OpenMenu("title");
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        foreach (Transform transform in roomListContent)
            Destroy(transform.gameObject);

        foreach (RoomInfo roomInfo in roomList)
            if (!roomInfo.RemovedFromList)
                Instantiate(roomListItemPrefab, roomListContent).GetComponent<RoomListItem>().Setup(roomInfo);
    }

    public void JoinRoom(RoomInfo info) {
        PhotonNetwork.JoinRoom(info.Name);
        MenuManager.instance.OpenMenu("loading");
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Instantiate(playerListItemPrefab, playerListContent).GetComponent<PlayerListItem>().Setup(newPlayer);
    }
}

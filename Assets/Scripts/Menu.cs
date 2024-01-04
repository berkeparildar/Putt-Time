using System.Collections.Generic;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;

public class Menu : MonoBehaviourPunCallbacks, IOnEventCallback
{

    private const string Characters = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
    private string[] levelNames = new string[] { "Game" };
    private const int roomNameLength = 5;
    [SerializeField] private TMP_InputField roomNameField;
    [SerializeField] private TMP_InputField playerNameField;
    [SerializeField] private string roomName;
    [SerializeField] private string playerName;
    [SerializeField] private string selectedLevelName;
    [SerializeField] private TextMeshProUGUI roomNameText;
    [SerializeField] private TextMeshProUGUI playerNameOne;
    [SerializeField] private TextMeshProUGUI playerNameTwo;
    [SerializeField] private TextMeshProUGUI playerNameThree;
    [SerializeField] private TextMeshProUGUI playerNameFour;
    [SerializeField] private List<TextMeshProUGUI> playerNamesList;

    void Start()
    {
        PhotonNetwork.ConnectUsingSettings();
        playerNameField.onEndEdit.AddListener(SetPlayerName);
        roomNameField.onEndEdit.AddListener(SetRoomName);
    }

    public void PlayButton()
    {

    }

    public void JoinRoom()
    {
        PhotonNetwork.JoinRoom(roomName);
    }

    public override void OnJoinedRoom()
    {
        int index = 1;
        playerNameOne.text = playerName;
        foreach (var playerEntry in PhotonNetwork.CurrentRoom.Players)
        {
            if (playerEntry.Value != PhotonNetwork.LocalPlayer && index <= 3)
            {
                playerNamesList[index].text = playerEntry.Value.NickName;
                index++;
            }
        }
    }

    public override void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer)
    {
        int index = 1;
        playerNameOne.text = playerName;
        foreach (var playerEntry in PhotonNetwork.CurrentRoom.Players)
        {
            if (playerEntry.Value != PhotonNetwork.LocalPlayer && index <= 3)
            {
                playerNamesList[index].text = playerEntry.Value.NickName;
                index++;
            }
        }
    }


    public void StartGame()
    {
        GameManager.RaiseEventToAll(GameManager.GetEventCode(EventCode.SETLEVEL), selectedLevelName);
    }

    public void CreateRoom()
    {
        var generatedRoomName = GenerateRoomName();
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = 4;
        PhotonNetwork.CreateRoom(generatedRoomName, roomOptions);
        selectedLevelName = SetLevel();
        roomNameText.text = generatedRoomName;
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("Connected to server.");
        PhotonNetwork.JoinLobby();
    }

    public string GenerateRoomName()
    {
        char[] roomName = new char[roomNameLength];
        System.Random random = new System.Random();

        for (int i = 0; i < roomNameLength; i++)
        {
            roomName[i] = Characters[random.Next(Characters.Length)];
        }

        return new string(roomName);
    }

    public void SetRoomName(string name)
    {
        roomName = name;
    }

    public void SetPlayerName(string name)
    {
        playerName = name;
    }

    public void SubmitPlayerName()
    {
        PhotonNetwork.NickName = playerName;
    }

    public string SetLevel()
    {
        string levelName = levelNames[Random.Range(0, levelNames.Length)];
        return levelName;
    }

    public void OnEvent(EventData photonEvent)
    {
        if (photonEvent.Code == GameManager.GetEventCode(EventCode.SETLEVEL))
        {
            selectedLevelName = (string)photonEvent.CustomData;
            PhotonNetwork.LoadLevel(selectedLevelName);
        }
    }
}
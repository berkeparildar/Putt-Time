using System.Collections.Generic;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Menu : MonoBehaviourPunCallbacks, IOnEventCallback
{
    private const string Characters = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
    private readonly string[] _levelNames = new string[] { "Game" };
    private const int RoomNameLength = 5;
    [SerializeField] private TMP_InputField roomNameField;
    [SerializeField] private TMP_InputField playerNameField;
    [SerializeField] private string roomName;
    [SerializeField] private string playerName;
    [SerializeField] private string selectedLevelName;
    [SerializeField] private TextMeshProUGUI roomNameText;
    [SerializeField] private TextMeshProUGUI playerNameOne;
    [SerializeField] private List<TextMeshProUGUI> playerNamesList;
    [SerializeField] private Button startButton;
    [SerializeField] private Animator canvasAnimator;
    private static readonly int GoToNameEntry = Animator.StringToHash("GoToNameEntry");
    private static readonly int GoToRoomButtons = Animator.StringToHash("GoToRoomButtons");
    private static readonly int NameToStart = Animator.StringToHash("NameToStart");
    private static readonly int CreateToFinal = Animator.StringToHash("CreateToFinal");
    private static readonly int JoinRoom1 = Animator.StringToHash("JoinRoom");
    private static readonly int RoomToStart = Animator.StringToHash("RoomToStart");
    private static readonly int JoinToFinal = Animator.StringToHash("JoinToFinal");
    private static readonly int JoinToRoom = Animator.StringToHash("JoinToRoom");
    private static readonly int FinalToRoom = Animator.StringToHash("FinalToRoom");

    void Start()
    {
        PhotonNetwork.ConnectUsingSettings();
        playerNameField.onEndEdit.AddListener(SetPlayerName);
        roomNameField.onEndEdit.AddListener(SetRoomName);
    }

    public void PlayButton()
    {
        if (playerName.Length >= 2)
        {
            canvasAnimator.SetTrigger(GoToRoomButtons);
        }
        else
        {
            canvasAnimator.SetTrigger(GoToNameEntry);
        }
    }

    public void JoinRoom()
    {
        PhotonNetwork.JoinRoom(roomName);
        startButton.interactable = false;
        roomNameText.text = "Room ID: " + roomName;
        startButton.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "Waiting..";
        canvasAnimator.SetTrigger(JoinToFinal);
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

    public void QuitApp()
    {
        PhotonNetwork.LeaveLobby();
        PhotonNetwork.Disconnect();
        Application.Quit();
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
        roomNameText.text = "Room ID: " + generatedRoomName;
        canvasAnimator.SetTrigger(CreateToFinal);
    }

    public void GoToRoomName()
    {
        canvasAnimator.SetTrigger(JoinRoom1);
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("Connected to server.");
        PhotonNetwork.JoinLobby();
    }

    public string GenerateRoomName()
    {
        char[] roomName = new char[RoomNameLength];
        System.Random random = new System.Random();

        for (int i = 0; i < RoomNameLength; i++)
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

    public void NameEntryBack()
    {
        canvasAnimator.SetTrigger(NameToStart);
    }

    public void RoomButtonsBack()
    {
        canvasAnimator.SetTrigger(RoomToStart);
    }

    public void RoomNameBack()
    {
        canvasAnimator.SetTrigger(JoinToRoom);
    }

    public void FinalBack()
    {
        PhotonNetwork.LeaveRoom();
        canvasAnimator.SetTrigger(FinalToRoom);
    }

    public void SubmitPlayerName()
    {
        PhotonNetwork.NickName = playerName;
        canvasAnimator.SetTrigger(GoToRoomButtons);
    }

    public string SetLevel()
    {
        string levelName = _levelNames[Random.Range(0, _levelNames.Length)];
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
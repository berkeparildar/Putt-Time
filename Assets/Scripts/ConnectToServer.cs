using Photon.Pun;
using UnityEngine;

public class ConnectToServer : MonoBehaviourPunCallbacks
{
    private bool _joinedRoom;
    private bool _hasLoaded;

    void Start()
    {
        PhotonNetwork.ConnectUsingSettings();
    }

    void Update(){
        if (_joinedRoom && PhotonNetwork.InRoom)
        {
            if (PhotonNetwork.CurrentRoom.PlayerCount == 2 && !_hasLoaded)
            {
                _hasLoaded = true;
                PhotonNetwork.LoadLevel("Game");
            }
        }
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("Connected to server.");
        PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby()
    {
        Debug.Log("Joined lobby.");
        Debug.Log(PhotonNetwork.IsMasterClient);
        if (PhotonNetwork.CountOfRooms == 0)
        {
            Debug.Log("No rooms, creating.");
            PhotonNetwork.CreateRoom("PHOTON");
        }
        else
        {
            Debug.Log("Joining room");
            PhotonNetwork.JoinRoom("PHOTON");
        }
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("Joined Room");
        _joinedRoom = true;
    }

    public override void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer)
    {
        Debug.Log("Player entered room");
    }
}

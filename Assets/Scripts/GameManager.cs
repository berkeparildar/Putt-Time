using Photon.Pun;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    void Start() { }

    private void OnEnable()
    {
        Player.InHole += GoToNextHole;
    }

    private void OnDisable()
    {
        Player.InHole -= GoToNextHole;
    }

    void Update()
    {
        Debug.Log(PhotonNetwork.CurrentRoom.PlayerCount);
    }

    private void GoToNextHole()
    {
        Debug.Log("Event");
    }
}

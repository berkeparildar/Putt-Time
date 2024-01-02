using Photon.Pun;
using UnityEngine;

public class SpawnPlayers : MonoBehaviour
{

    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private Vector3 spawnPosition;

    void Start()
    {
        Debug.Log("Creating player!!");
        PhotonNetwork.Instantiate(playerPrefab.name, spawnPosition, Quaternion.identity);
        Debug.Log(PhotonNetwork.CurrentRoom.PlayerCount);
    }
}

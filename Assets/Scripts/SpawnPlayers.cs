using Photon.Pun;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SpawnPlayers : MonoBehaviour
{

    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private Vector3 spawnPosition;
    [SerializeField] private GameObject jumper;

    void Start()
    {
        Debug.Log("Creating player!!");
        PhotonNetwork.Instantiate(playerPrefab.name, spawnPosition, Quaternion.identity);
        if (PhotonNetwork.IsMasterClient)
        {
            SpawnObjects();
        }   
    }

    private void SpawnObjects()
    {
        if (SceneManager.GetActiveScene().name == "Jumpington")
        {
            PhotonNetwork.Instantiate(jumper.name, new Vector3(0, 0, 15.26f), Quaternion.identity);
            PhotonNetwork.Instantiate(jumper.name, new Vector3(40, 0, 125), Quaternion.identity);
            PhotonNetwork.Instantiate(jumper.name, new Vector3(51.5f, 0, 50), Quaternion.identity);
            PhotonNetwork.Instantiate(jumper.name, new Vector3(51.5f, 6, 25), Quaternion.identity);
            PhotonNetwork.Instantiate(jumper.name, new Vector3(75, 0, 50), Quaternion.identity);
            PhotonNetwork.Instantiate(jumper.name, new Vector3(-64.36f, 0, 44.32f), Quaternion.identity);
            PhotonNetwork.Instantiate(jumper.name, new Vector3(-50, 0, 150), Quaternion.identity);
            PhotonNetwork.Instantiate(jumper.name, new Vector3(170, 0, 25), Quaternion.identity);
        }
    }
}

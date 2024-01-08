using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SpawnPlayers : MonoBehaviour
{

    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private Vector3 spawnPosition;
    [SerializeField] private GameObject jumper;
    [SerializeField] private GameObject spinnerSmall;
    [SerializeField] private GameObject spinnerT;
    [SerializeField] private GameObject spinnerTriangle;
    [SerializeField] private GameObject spinnerX;

    void Awake()
    {
        PhotonNetwork.Instantiate(playerPrefab.name, spawnPosition, Quaternion.identity);
        if (PhotonNetwork.IsMasterClient)
        {
            SpawnObjects();
        }  
         
    }
    private void AssignPlayerMaterials()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            Dictionary<string, int> colorDictionary = new Dictionary<string, int>();
            int index = 0;
            foreach (var player in PhotonNetwork.CurrentRoom.Players)
            {
                Debug.Log("Added to dict" + player.Value.NickName);
                colorDictionary.Add(player.Value.NickName, index);
                index++;
            }
            var data = colorDictionary;
            GameManager.RaiseEventToAll(GameManager.GetEventCode(EventCode.SETMATERIAL), data);
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
        else if (SceneManager.GetActiveScene().name == "Spinnington")
        {
            PhotonNetwork.Instantiate(spinnerSmall.name, new Vector3(-14.05f,68.71f,75.36f), Quaternion.identity);
            PhotonNetwork.Instantiate(spinnerT.name, new Vector3(85,72,115), Quaternion.identity);
            PhotonNetwork.Instantiate(spinnerT.name, new Vector3(85,72,85), Quaternion.identity);
            PhotonNetwork.Instantiate(spinnerT.name, new Vector3(126.93f,75.84f,182.2f), Quaternion.identity);
            PhotonNetwork.Instantiate(spinnerT.name, new Vector3(-15.58f,72.5f,217.7f), Quaternion.identity);
            PhotonNetwork.Instantiate(spinnerT.name, new Vector3(15.5f,72.5f,250), Quaternion.identity);
            PhotonNetwork.Instantiate(spinnerX.name, new Vector3(232.3f,72.5f,57), Quaternion.identity);
            PhotonNetwork.Instantiate(spinnerX.name, new Vector3(194.7f,72.5f,98.6f), Quaternion.identity);
            PhotonNetwork.Instantiate(spinnerTriangle.name, new Vector3(110.5f,68.28f,22.3f), Quaternion.identity);
            PhotonNetwork.Instantiate(spinnerTriangle.name, new Vector3(110.5f,68.28f,2.5f), Quaternion.identity);
            PhotonNetwork.Instantiate(spinnerTriangle.name, new Vector3(15.5f,72,217.9f), Quaternion.identity);
            PhotonNetwork.Instantiate(spinnerTriangle.name, new Vector3(-17.5f,72,258.8f), Quaternion.identity);
        }
    }
}

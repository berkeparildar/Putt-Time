using System;
using System.Collections.Generic;
using Cinemachine;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class Player : MonoBehaviour, IOnEventCallback
{

    [SerializeField] private Vector3 spawnPosition;
    [SerializeField] private PlayerBall playerBall;
    [SerializeField] private Rigidbody rb;
    [SerializeField] private MeshRenderer playerRenderer;
    [SerializeField] private List<Material> playerMaterials;
    [SerializeField] private PhotonView photonView;

    private CinemachineVirtualCamera _virtualCamera;
    private GameManager _gameManager;

    public static Action<Player> PlayerJoined;
    public static Action MovedToNextHole;
    public static Action ScoreUpdated;

    private bool _inHole;
    private int _stroke;
    public int score;
    public string playerName;

    void Start()
    {
        playerBall = transform.GetChild(0).GetComponent<PlayerBall>();
        _gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        AssignPlayerMaterial();
        SetUpCamera();
        if (photonView.IsMine)
        {
            playerName = PhotonNetwork.NickName;
        }
        else
        {
            playerName = photonView.Owner.NickName;
        }
        PlayerJoined?.Invoke(this);
    }

    private void OnEnable()
    {
        PhotonNetwork.AddCallbackTarget(this);
    }

    private void OnDisable()
    {
        PhotonNetwork.RemoveCallbackTarget(this);
    }

    private void SetUpCamera()
    {
        if (photonView.IsMine)
        {
            _virtualCamera = GameObject
                .Find("VirtualCamera")
                .GetComponent<CinemachineVirtualCamera>();
            _virtualCamera.m_Follow = transform;
            _virtualCamera.m_LookAt = transform;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (photonView.IsMine)
        {
            if (other.CompareTag("Hole") && !_inHole)
            {
                _inHole = true;
                GameManager.RaiseEventToAll(GameManager.GetEventCode(EventCode.INHOLE), null);
            }
            else if (other.CompareTag("RespawnPlane"))
            {
                rb.velocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
                transform.position = playerBall.GetLastSavedPosition();
            }
        }
    }

    public void OnEvent(EventData photonEvent)
    {
        if (photonView.IsMine)
        {
            if (photonEvent.Code == GameManager.GetEventCode(EventCode.NEXTHOLE))
            {
                score += _gameManager.CalculateScore(_stroke);
                var scoreData = new object[] { photonView.ViewID, score };
                GameManager.RaiseEventToAll(GameManager.GetEventCode(EventCode.UPDATESCORE), scoreData);
                var nextHolePosition = (Vector3)photonEvent.CustomData;
                ResetPlayerVelocity();
                transform.position = nextHolePosition;
                _stroke = 0;
                playerBall.SetLastSavedPosition(nextHolePosition);
                MovedToNextHole?.Invoke();
                ScoreUpdated?.Invoke();
            }
        }
        if (!photonView.IsMine && photonEvent.Code == GameManager.GetEventCode(EventCode.UPDATESCORE))
        {
            var data = (object[])photonEvent.CustomData;
            var viewId = (int)data[0];
            if (viewId == photonView.ViewID)
            {
                score += (int)data[1];
            }
            ScoreUpdated?.Invoke();
        }
    }

    private void ResetPlayerVelocity()
    {
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
    }

    public void IncreaseStroke()
    {
        _stroke++;
    }

    private void AssignPlayerMaterial()
    {
        if (photonView.IsMine)
        {
            playerRenderer.material = playerMaterials[playerMaterials.Count - 1];
            playerRenderer.material.renderQueue = 2500;
            playerMaterials.RemoveAt(playerMaterials.Count - 1);
            transform.localScale = new Vector3(0.51f, 0.51f, 0.51f);
        }
        else
        {
            playerRenderer.material = playerMaterials[0];
            playerMaterials.RemoveAt(0);
        }
    }
}

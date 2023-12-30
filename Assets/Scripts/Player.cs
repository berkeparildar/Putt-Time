using System;
using Cinemachine;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class Player : MonoBehaviour, IOnEventCallback
{
    [SerializeField] private Vector3 spawnPosition;
    private CinemachineVirtualCamera _virtualCamera;
    private PhotonView _photonView;
    private bool _inHole;

    void Start()
    {
        _photonView = GetComponent<PhotonView>();
        SetUpCamera();
    }

     private void OnEnable()
    {
        PhotonNetwork.AddCallbackTarget(this);
    }

    private void OnDisable()
    {
        PhotonNetwork.RemoveCallbackTarget(this);
    }

    private void Update() { }

    private void SetUpCamera()
    {
        if (_photonView.IsMine)
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
        if (other.CompareTag("Hole") && !_inHole)
        {
            _inHole = true;
            GameManager.RaiseEventToAll(GameManager.GetEventCode(EventCode.INHOLE), null);
        }
    }

    public void OnEvent(EventData photonEvent)
    {
        if (_photonView.IsMine)
        {
            if (photonEvent.Code == GameManager.GetEventCode(EventCode.NEXTHOLE))
            {
                transform.position = (Vector3)photonEvent.CustomData;
            }
        }
    }
}

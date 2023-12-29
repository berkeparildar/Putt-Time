using System;
using Cinemachine;
using Photon.Pun;
using UnityEngine;

public class Player : MonoBehaviour
{
    public static Action InHole;

    [SerializeField]
    private Vector3 spawnPosition;

    private CinemachineVirtualCamera _virtualCamera;

    private PhotonView _photonView;

    void Start()
    {
        _photonView = GetComponent<PhotonView>();
        SetUpCamera();
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
        if (other.CompareTag("Hole"))
        {
            InHole?.Invoke();
        }
    }
}

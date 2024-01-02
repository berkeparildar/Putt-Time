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
    private bool _inHole;
    private int _stroke;

    void Start()
    {
        playerBall = transform.GetChild(0).GetComponent<PlayerBall>();
        AssignPlayerMaterial();
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

    public void OnEvent(EventData photonEvent)
    {
        if (photonView.IsMine)
        {
            if (photonEvent.Code == GameManager.GetEventCode(EventCode.NEXTHOLE))
            {
                var nextHolePosition = (Vector3)photonEvent.CustomData; 
                transform.position = nextHolePosition;
                _stroke = 0;
                playerBall.SetLastSavedPosition(nextHolePosition);
            }
        }
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
            playerMaterials.RemoveAt(playerMaterials.Count - 1);
        }
        else
        {
            playerRenderer.material = playerMaterials[0];
            playerMaterials.RemoveAt(0);
        }
    }
}

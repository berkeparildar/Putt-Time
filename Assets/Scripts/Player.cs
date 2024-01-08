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
    [SerializeField] private float jumpStrength;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip audioClip;

    private CinemachineVirtualCamera _virtualCamera;
    private GameManager _gameManager;

    public static Action<Player> PlayerJoined;
    public static Action MovedToNextHole;
    public static Action<string, Color> OnColorSet;

    [SerializeField] private bool inHole;
    [SerializeField] private int stroke;
    public Color color;
    public int score;
    public string playerName;

    void Start()
    {
        Debug.Log(PhotonNetwork.LocalPlayer.NickName);
        playerBall = transform.GetChild(0).GetComponent<PlayerBall>();
        _gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
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
            if (other.CompareTag("Hole") && !inHole)
            {
                audioSource.PlayOneShot(audioClip);
                inHole = true;
                score += _gameManager.CalculateScore(stroke);
                var scoreData = new object[] { PhotonNetwork.LocalPlayer.NickName, score };
                GameManager.RaiseEventToAll(GameManager.GetEventCode(EventCode.INHOLE), scoreData);
            }
            else if (other.CompareTag("RespawnPlane"))
            {
                rb.velocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
                transform.position = playerBall.GetLastSavedPosition();
            }
            else if (other.CompareTag("Jumper"))
            {
                audioSource.PlayOneShot(audioClip);
                other.transform.parent.GetComponent<Animator>().SetTrigger("Jump");
                rb.AddForce(Vector3.up * jumpStrength, ForceMode.Impulse);
            }
        }
    }

    public void OnEvent(EventData photonEvent)
    {
        if (photonView.IsMine)
        {
            if (photonEvent.Code == GameManager.GetEventCode(EventCode.NEXTHOLE))
            {
                var nextHolePosition = (Vector3)photonEvent.CustomData;
                ResetPlayerVelocity();
                transform.position = nextHolePosition;
                stroke = 0;
                playerBall.SetLastSavedPosition(nextHolePosition);
                MovedToNextHole?.Invoke();
                inHole = false;
            }
            else if (photonEvent.Code == GameManager.GetEventCode(EventCode.SETMATERIAL))
            {
                var data = (Dictionary<string, int>)photonEvent.CustomData;
                var colorIndex = data[PhotonNetwork.LocalPlayer.NickName];
                playerRenderer.material = playerMaterials[colorIndex];
                color = playerMaterials[colorIndex].color;
                OnColorSet?.Invoke(PhotonNetwork.LocalPlayer.NickName, color);
                playerRenderer.material.renderQueue = 2500;
                transform.localScale = new Vector3(1.05f, 1.05f, 1.05f);
            }
        }
        else
        {
            if (photonEvent.Code == GameManager.GetEventCode(EventCode.SETMATERIAL))
            {
                Debug.Log(photonView.Owner.NickName);
                var data = (Dictionary<string, int>)photonEvent.CustomData;
                var colorIndex = data[photonView.Owner.NickName];
                color = playerMaterials[colorIndex].color;
                playerRenderer.material = playerMaterials[colorIndex];
                OnColorSet?.Invoke(photonView.Owner.NickName, color);
            }
        }
    }

    private void ResetPlayerVelocity()
    {
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
    }

    public void IncreaseStroke()
    {
        audioSource.PlayOneShot(audioClip);
        stroke++;
    }
}

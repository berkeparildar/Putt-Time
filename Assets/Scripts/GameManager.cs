using System.Collections;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public enum EventCode
{
    INHOLE,
    NEXTHOLE,
    UPDATESCORE,
    SETLEVEL
}

public class GameManager : MonoBehaviour, IOnEventCallback
{
    private int _currentHole;
    private LevelInitializer _levelInitializer;
    private const int MaxNumberOfHoles = 6;
    public int playersInHole;

    private void Start()
    {
        _levelInitializer = GetComponent<LevelInitializer>();
        _currentHole = 0;
    }

    private void OnEnable()
    {
        PhotonNetwork.AddCallbackTarget(this);
    }

    private void OnDisable()
    {
        PhotonNetwork.RemoveCallbackTarget(this);
    }

    public void OnEvent(EventData photonEvent)
    {
        if (PhotonNetwork.IsMasterClient && photonEvent.Code == GetEventCode(EventCode.INHOLE))
        {
            playersInHole++;
        }
    }

    private void Update()
    {
        CheckIfNextHole();
    }

    public static byte GetEventCode(EventCode eventCode)
    {
        byte code = 0;
        switch (eventCode)
        {
            case EventCode.INHOLE:
                code = 0;
                break;
            case EventCode.NEXTHOLE:
                code = 1;
                break;
            case EventCode.UPDATESCORE:
                code = 2;
                break;
            case EventCode.SETLEVEL:
                code = 3;
                break;
        }

        return code;
    }

    public static void RaiseEventToAll(byte code, object data)
    {
        var eventOptions = RaiseEventOptions.Default;
        eventOptions.Receivers = ReceiverGroup.All;
        PhotonNetwork.RaiseEvent(code, data, eventOptions, SendOptions.SendReliable);
    }

    private IEnumerator GoToNextHoleEvent()
    {
        yield return new WaitForSeconds(3);
        _currentHole++;
        RaiseEventToAll(GetEventCode(EventCode.NEXTHOLE),
            _levelInitializer.LevelHoles[_currentHole].GetSpawnPosition());
    }

    private void CheckIfNextHole()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            if (playersInHole == PhotonNetwork.CurrentRoom.PlayerCount)
            {
                playersInHole = 0;
                StartCoroutine(GoToNextHoleEvent());
            }
        }
    }

    public int CalculateScore(int stroke)
    {
        var currentPar = _levelInitializer.LevelHoles[_currentHole].GetParCount();
        var score = stroke - currentPar;
        return score;
    }
}
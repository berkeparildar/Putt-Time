using System.Collections;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Pun.UtilityScripts;
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

    public static byte GetEventCode(EventCode eventcode)
    {
        byte code = 0;
        switch (eventcode)
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
        Debug.Log("Sending event");
        var eventOpitons = RaiseEventOptions.Default;
        eventOpitons.Receivers = ReceiverGroup.All;
        PhotonNetwork.RaiseEvent(code, data, eventOpitons, SendOptions.SendReliable);
    }

    private IEnumerator GoToNextHoleEvent()
    {
        yield return new WaitForSeconds(3);
        _currentHole++;
        RaiseEventToAll(GetEventCode(EventCode.NEXTHOLE), _levelInitializer.levelHoles[_currentHole].GetSpawnPosition());
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
        var currentPar = _levelInitializer.levelHoles[_currentHole].GetParCount();
        var score = stroke - currentPar;
        return score;
    }
}

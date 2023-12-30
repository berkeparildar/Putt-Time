using System.Collections;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public enum EventCode
{
    INHOLE = 0,
    NEXTHOLE = 1
}

public class GameManager : MonoBehaviour, IOnEventCallback
{
    private int _currentHole;
    private const int MaxNumberOfHoles = 6;
    public int playersInHole;
    private List<GameObject> players;

    private void Start()
    {
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
        if (PhotonNetwork.IsMasterClient && photonEvent.Code == 0)
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
        RaiseEventToAll(GetEventCode(EventCode.NEXTHOLE), new Vector3(0, 0, 0));
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
}

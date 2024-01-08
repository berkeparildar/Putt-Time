using System.Collections;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public enum EventCode
{
    INHOLE,
    NEXTHOLE,
    UPDATESCORE,
    SETLEVEL,
    SETMATERIAL,
    END
}

public class GameManager : MonoBehaviour, IOnEventCallback
{
    [SerializeField] private int currentHole;
    private LevelInitializer _levelInitializer;
    private const int NumberOfHoles = 6;
    public int playersInHole;
    public Dictionary<string, int> scoreDictionary;

    private void Start()
    {
        _levelInitializer = GetComponent<LevelInitializer>();
        currentHole = 0;
        AssignPlayerMaterials();
        scoreDictionary = new Dictionary<string, int>();
        InitializeScoreDictionary();
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
            var data = (object[])photonEvent.CustomData;
            var userId = (string)data[0];
            scoreDictionary[userId] = (int)data[1];
            if (playersInHole == PhotonNetwork.CurrentRoom.PlayerCount)
            {
                playersInHole = 0;
                StartCoroutine(GoToNextHoleEvent());
            }
        }
    }

    private void InitializeScoreDictionary()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            foreach (var player in PhotonNetwork.CurrentRoom.Players)
            {
                scoreDictionary.Add(player.Value.UserId, 0);
            }
        }
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
            case EventCode.SETMATERIAL:
                code = 4;
                break;
            case EventCode.END:
                code = 5;
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
        currentHole++;
        if (currentHole == NumberOfHoles)
        {
            if (PhotonNetwork.IsMasterClient)
            {
                RaiseEventToAll(GetEventCode(EventCode.END), scoreDictionary);
            }
        }
        else
        {
            RaiseEventToAll(GetEventCode(EventCode.UPDATESCORE), scoreDictionary);
            RaiseEventToAll(GetEventCode(EventCode.NEXTHOLE),
            _levelInitializer.LevelHoles[currentHole].GetSpawnPosition());
        }
    }

    public int CalculateScore(int stroke)
    {
        var currentPar = _levelInitializer.LevelHoles[
currentHole].GetParCount();
        var score = stroke - currentPar;
        return score;
    }

    private void AssignPlayerMaterials()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            Dictionary<string, int> colorDictionary = new Dictionary<string, int>();
            int index = 0;
            foreach (var player in PhotonNetwork.CurrentRoom.Players)
            {
                colorDictionary.Add(player.Value.UserId, index);
                index++;
            }
            var data = colorDictionary;
            RaiseEventToAll(GetEventCode(EventCode.SETMATERIAL), data);
        }
    }
}
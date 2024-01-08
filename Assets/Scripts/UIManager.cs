using System.Collections.Generic;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : MonoBehaviour, IOnEventCallback
{

    [SerializeField] private List<Player> players;
    [SerializeField] private List<Player> sortedPlayers;

    [SerializeField] private TextMeshProUGUI holeText;
    [SerializeField] private TextMeshProUGUI strokeText;
    [SerializeField] private TextMeshProUGUI parText;
    [SerializeField] private TextMeshProUGUI timerText;

    [SerializeField] private List<TextMeshProUGUI> playerNames;
    [SerializeField] private List<TextMeshProUGUI> playerScores;
    [SerializeField] private List<TextMeshProUGUI> playerHoles;
    [SerializeField] private List<Image> playerColors;

    private int _stroke;
    private int _par;
    private int _hole;
    private int _numberOfPlayers;

    [SerializeField] private Sprite upArrow;
    [SerializeField] private Sprite downArrow;
    [SerializeField] private bool scoreShown;
    [SerializeField] private Animator uiAnimator;
    [SerializeField] private Image scoreCardButtonImage;

    [SerializeField] private TextMeshProUGUI winnerName;
    [SerializeField] private Image winnerColor;


    private void OnEnable()
    {
        Player.PlayerJoined += AddPlayer;
        Player.MovedToNextHole += NextHole;
        PlayerBall.UpdateStrokeUI += IncreaseStroke;
        Player.OnColorSet += InitializeUserColor;
        PhotonNetwork.AddCallbackTarget(this);

    }

    private void OnDisable()
    {
        Player.PlayerJoined -= AddPlayer;
        Player.MovedToNextHole -= NextHole;
        PlayerBall.UpdateStrokeUI -= IncreaseStroke;
        Player.OnColorSet -= InitializeUserColor;
        PhotonNetwork.RemoveCallbackTarget(this);
    }

    private void Start()
    {
        _stroke = 0;
        _hole = 1;
        _par = GetComponent<LevelInitializer>().LevelHoles[_hole - 1].GetParCount();
        UpdateUI();
    }

    private void UpdateUI()
    {
        strokeText.text = "Stroke: " + _stroke.ToString();
        parText.text = "Par: " + _par.ToString();
        holeText.text = "Hole: " + _hole.ToString();
    }

    private void AddPlayer(Player player)
    {
        _numberOfPlayers++;
        players.Add(player);
        if (_numberOfPlayers == PhotonNetwork.CurrentRoom.PlayerCount)
        {
            InitializeCard();
        }
    }

    public void IncreaseStroke()
    {
        _stroke++;
        strokeText.text = "Stroke: " + _stroke.ToString();
    }

    public void NextHole()
    {
        _stroke = 0;
        _hole++;
        _par = GetComponent<LevelInitializer>().LevelHoles[_hole - 1].GetParCount();
        UpdateUI();
    }

    private void SortPlayers()
    {
        sortedPlayers = new List<Player>(players.Count);
        for (int i = 0; i < players.Count; i++)
        {
            var sc = int.MaxValue;
            var playerIndex = -1;
            for (int j = 0; j < players.Count; j++)
            {
                if (!sortedPlayers.Contains(players[j]) && players[j].score <= sc)
                {
                    playerIndex = j;
                    sc = players[j].score;
                }
            }
            if (playerIndex != -1)
            {
                sortedPlayers.Add(players[playerIndex]);
            }
        }
    }

    private void UpdateCard()
    {
        for (int i = 0; i < sortedPlayers.Count; i++)
        {
            playerNames[i].text = sortedPlayers[i].playerName;
            playerScores[i].text = sortedPlayers[i].score.ToString();
            playerColors[i].color = sortedPlayers[i].color;
            playerHoles[i].text = _hole.ToString();
              if (sortedPlayers[i].GetComponent<PhotonView>().IsMine)
            {
                playerNames[i].transform.parent.GetChild(0).gameObject.SetActive(true);
            }
            else
            {
                playerNames[i].transform.parent.GetChild(0).gameObject.SetActive(false);
            }
        }
    }

    private void InitializeCard()
    {
        for (int i = 0; i < players.Count; i++)
        {
            if (players[i].GetComponent<PhotonView>().IsMine)
            {
                playerNames[i].transform.parent.GetChild(0).gameObject.SetActive(true);
            }
            playerNames[i].text = players[i].playerName;
            playerScores[i].text = "0";
            playerHoles[i].text = "1";
        }
    }

    private void InitializeUserColor(string id, Color color)
    {
        for (int i = 0; i < players.Count; i++)
        {
            if (players[i].GetComponent<PhotonView>().Owner.UserId == id)
            {
                playerColors[i].color = color;
            }
        }
    }

    public void OnEvent(EventData photonEvent)
    {
        if (photonEvent.Code == GameManager.GetEventCode(EventCode.UPDATESCORE))
        {
            var scoreData = (Dictionary<string, int>)photonEvent.CustomData;
            foreach (var player in players)
            {
                player.score = scoreData[player.GetComponent<PhotonView>().Owner.UserId];
            }
            SortPlayers();
            UpdateCard();
        }
        else if (photonEvent.Code == GameManager.GetEventCode(EventCode.END))
        {
            var scoreData = (Dictionary<string, int>)photonEvent.CustomData;
            foreach (var player in players)
            {
                player.score = scoreData[player.GetComponent<PhotonView>().Owner.UserId];
            }
            SortPlayers();
            UpdateCard();
            winnerName.text = sortedPlayers[0].playerName;
            winnerColor.color = sortedPlayers[0].color;
            uiAnimator.SetTrigger("End");
        }
    }

    public void PlayAgainButton()
    {
        var levelNames = new string[] {"Classicton", "Spinnington", "Jumpington"};
        PhotonNetwork.LoadLevel(levelNames[Random.Range(0, levelNames.Length)]);
    }

    public void GoToMenuButton()
    {
        PhotonNetwork.LeaveRoom();
        SceneManager.LoadScene(0);
    }

    public void ScoreCard()
    {
        if (!scoreShown)
        {
            scoreShown = true;
            uiAnimator.SetTrigger("ShowCard");
            scoreCardButtonImage.sprite = downArrow;
        }
        else
        {
            scoreShown = false;
            uiAnimator.SetTrigger("CloseCard");
            scoreCardButtonImage.sprite = upArrow;
        }
    }
}

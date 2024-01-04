using System.Collections.Generic;
using Photon.Pun;
using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
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


    private int _stroke;
    private int _par;
    private int _hole;
    private int _numberOfPlayers;
    private int _updatedScores;

    private void OnEnable()
    {
        Player.PlayerJoined += AddPlayer;
        Player.MovedToNextHole += NextHole;
        Player.ScoreUpdated += UpdateScores;
        PlayerBall.UpdateStrokeUI += IncreaseStroke;
    }

    private void OnDisable()
    {
        Player.PlayerJoined -= AddPlayer;
        Player.MovedToNextHole -= NextHole;
        Player.ScoreUpdated -= UpdateScores;
        PlayerBall.UpdateStrokeUI -= IncreaseStroke;
    }

    void Start()
    {
        _stroke = 0;
        _hole = 1;
        _par = GetComponent<LevelInitializer>().levelHoles[_hole - 1].GetParCount();
        UpdateUI();
    }

    private void UpdateUI()
    {
        strokeText.text = _stroke.ToString();
        parText.text = _par.ToString();
        holeText.text = _hole.ToString();
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
        strokeText.text = _stroke.ToString();
    }

    public void NextHole()
    {
        _stroke = 0;
        _hole++;
        _par = GetComponent<LevelInitializer>().levelHoles[_hole - 1].GetParCount();
        UpdateUI();
    }

    private void UpdateScores()
    {
        Debug.Log(_updatedScores + "Called");
        _updatedScores++;
        if (_updatedScores == players.Count)
        {
            Debug.Log("In if ");
            SortPlayers();
            UpdateCard();
            _updatedScores = 0;
        }
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
                Debug.Log("Lowest score is: " + players[playerIndex].score);
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
            playerHoles[i].text = _hole.ToString();
        }
    }
    
    private void InitializeCard()
    {
        for (int i = 0; i < players.Count; i++)
        {
            playerNames[i].text = players[i].playerName;
            playerScores[i].text = "0";
            playerHoles[i].text = "1";
        }
    }
}

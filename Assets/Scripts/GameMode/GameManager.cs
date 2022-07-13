using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Mirror;

public class GameManager : NetworkBehaviour
{

    public static GameManager Instance = null;
    private GameRoundManager gameRoundManager;
    [Header("Player")]
    public GameObject playerOne;
    public GameObject playerTwo;

    public Transform playerOneSpawnPosition;
    public Transform playerTwoSpawnPosition;


    [Header("Comsumables")]
    public List<Wearable> hats;




    [Header("Game Stats")]
    public int teamOneRoundsWon;
    public int teamTwoRoundsWon;
    public TMP_Text blueTeamScore;
    public TMP_Text redTeamScore;
    


    [Header("Game State")]
    public bool roundGoing;
    public int currentRound;
    public bool gameStarted;
    public TMP_Text gameOverText;
    public TMP_Text roundStatus;
    public TMP_Text roundTime;


    private NetworkPlayer[] players;

    double startTime;
    void Awake()
    {
        Instance = this;
        roundStatus.text = "Waiting for players";
        currentRound = 1;
        gameRoundManager = GetComponent<GameRoundManager>();
    }

    private void Update()
    {
        players = FindObjectsOfType<NetworkPlayer>();
        if (isServer)
        {
            if (players.Length >= 2 && !gameStarted)
            {
                RpcStartGame(NetworkTime.time);
            }
        }
    }


    [ClientRpc]
    void RpcStartGame(double startTime)
    {
        gameStarted = true;
        StartCoroutine(gameRoundManager.PreGame(startTime));
    }




    [Command(ignoreAuthority = true)]
    public void CmdGetCurrentGameState()
    {
        if (isServer)
        {
            RpcGetCurrentGameState(gameStarted, currentRound, roundGoing, startTime);
        }
    }

    [ClientRpc]
    public void RpcGetCurrentGameState(bool gameStarted, int currentRound, bool roundGoing, double startTime)
    {
        if(!isServer)
        {
            this.gameStarted = gameStarted;
            this.currentRound = currentRound;
            this.roundGoing = roundGoing;
            if(gameStarted)
            {
                StartCoroutine(gameRoundManager.PreGame(startTime));
            }
        }
    }

    public void RoundStart()
    {
        if(isServer)
        {
            startTime = NetworkTime.time;
            RpcStartRound(currentRound, startTime);
        }
    }

    [ClientRpc]
    void RpcStartRound(int currentRound, double startTime)
    {
        StartCoroutine(gameRoundManager.Round(currentRound, startTime));
    }
    public void RoundEnd()
    {
        if(isServer)
        {
            currentRound++;
            RpcStartPreRound(NetworkTime.time);
        }
    }

    public void PlayerDied()
    {

    }


    [ClientRpc]
    void RpcStartPreRound(double startTime)
    {
        gameStarted = true;
        StartCoroutine(gameRoundManager.PreGame(startTime));
    }










}
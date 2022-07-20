using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Mirror;

public class GameManager : NetworkBehaviour
{

    public static GameManager Instance = null;
    private GameRoundManager gameRoundManager;
    public int maxRounds;
    [Header("Comsumables")]
    public List<Wearable> hats;

    [Header("UI Text")]
    public TMP_Text roundStatus;
    public TMP_Text roundTime;

    public TMP_Text blueTeamScore;
    public TMP_Text redTeamScore;

    public GameObject gameOverScreen;
    public TMP_Text gameOverScreenText;

    [Header("Player")]
    public Transform playerOneSpawnPosition;
    public Transform playerTwoSpawnPosition;



    private int teamOneRoundsWon;
    private int teamTwoRoundsWon;

    private NetworkPlayer playerOne;
    private NetworkPlayer playerTwo;

    private NetworkPlayer[] players;
    private int currentRound;
    private bool gameStarted;
    private double startTime;


    void Awake()
    {
        Instance = this;
        roundStatus.text = "Waiting for players";
        currentRound = 1;
        gameRoundManager = GetComponent<GameRoundManager>();
    }

    public void StartGame()
    {
        if(isServer)
        {
            players = FindObjectsOfType<NetworkPlayer>();
            playerOne = players[0];
            playerOne.RpcSetTeam(0);
            playerTwo = players[1];
            playerTwo.RpcSetTeam(1);
            RpcStartGame(NetworkTime.time);
        }
    }

    [ClientRpc]
    void RpcStartGame(double startTime)
    {
        gameStarted = true;
        StartCoroutine(gameRoundManager.PreGame(startTime));
    }

    [Command(ignoreAuthority = true)]
    public void CmdPlayerDied(NetworkIdentity netId, int team)
    {
        RpcPlayerDied(netId, team);
    }



    [ClientRpc]
    void RpcPlayerDied(NetworkIdentity netId, int team)
    {
        if (team == 0)
        {
            teamTwoRoundsWon++;
            netId.GetComponent<CharacterController>().transform.position = playerOneSpawnPosition.position;
        }
        else
        {
            teamOneRoundsWon++;
            netId.GetComponent<CharacterController>().transform.position = playerTwoSpawnPosition.position;
        }
        if (currentRound == maxRounds)
        {
            GameEnd();
            return;
        }
        RoundEnd();
        blueTeamScore.text = "" + teamOneRoundsWon;
        redTeamScore.text = "" + teamTwoRoundsWon;
    }


    [Command(ignoreAuthority = true)]
    public void CmdGetCurrentGameState()
    {
        if (isServer)
        {
            RpcGetCurrentGameState(gameStarted, currentRound, startTime);
        }
    }

    [ClientRpc]
    public void RpcGetCurrentGameState(bool gameStarted, int currentRound, double startTime)
    {
        if (!isServer)
        {
            this.gameStarted = gameStarted;
            this.currentRound = currentRound;
            if (gameStarted)
            {
                StartCoroutine(gameRoundManager.PreGame(startTime));
            }
        }
    }

    public void RoundStart()
    {
        if (isServer)
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
        if (isServer)
        {
            if (currentRound == maxRounds)
            {
                GameEnd();
                return;
            }
            currentRound++;
            StopAllCoroutines();
            RpcStartPreRound(NetworkTime.time);
        }
    }

    public void GameEnd()
    {
        if (isServer)
        {
            playerOne.RpcDisablePlayer(playerOneSpawnPosition.position, playerOneSpawnPosition.rotation);
            playerTwo.RpcDisablePlayer(playerTwoSpawnPosition.position, playerTwoSpawnPosition.rotation);
            StopAllCoroutines();
            RpcFinishGame();
        }
    }

    [ClientRpc]
    void RpcStartPreRound(double startTime)
    {
        gameStarted = true;
        StartCoroutine(gameRoundManager.PreGame(startTime));
    }

    [ClientRpc]
    void RpcFinishGame()
    {
        StartCoroutine(EndOfGameDisplay());
    }

    IEnumerator EndOfGameDisplay()
    {
        if (teamOneRoundsWon > teamTwoRoundsWon)
        {
            ShowEndOfGameUI(0);
        }
        else if(teamOneRoundsWon < teamTwoRoundsWon)
        {
            ShowEndOfGameUI(1);
        }
        yield return new WaitForSeconds(5);
        Cursor.lockState = CursorLockMode.None;
        NetworkManager.singleton.ServerChangeScene(NetworkManager.singleton.offlineScene);
        NetworkManager.singleton.StopHost();
        NetworkManager.singleton.StopServer();
    }

    void ShowEndOfGameUI(int winningTeam)
    {
        gameOverScreen.SetActive(true);
        if (winningTeam == 0)
        {
            gameOverScreenText.text = "Player 1 Won Congrats!";
        }
        else
        {
            gameOverScreenText.text = "Player 2 Won Congrats!";
        }
    }

    public void EnablePlayers()
    {
        Debug.LogError("Enable Players");
        if (isServer)
        {
            playerOne.RpcEnablePlayer();
            playerTwo.RpcEnablePlayer();
        }

    }
    public void DisablePlayers()
    {
        Debug.LogError("Disable Players");
        if (isServer)
        {
            playerOne.RpcDisablePlayer(playerOneSpawnPosition.position, playerOneSpawnPosition.rotation);
            playerTwo.RpcDisablePlayer(playerTwoSpawnPosition.position, playerTwoSpawnPosition.rotation);
        }
    }










}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Mirror;

public class GameManager : NetworkBehaviour
{
    [Header("Game Settings")]
    public static GameManager Instance = null;
    private GameRoundManager gameRoundManager;
    public int maxRounds;

    private int teamOneRoundsWon;
    private int teamTwoRoundsWon;

    private int currentRound;
    private bool gameStarted;
    private double startTime;


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

    private NetworkPlayer playerOne;
    private NetworkPlayer playerTwo;

    private NetworkPlayer[] players;



    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }
        //Initialize game settings
        roundStatus.text = "Waiting for players";
        currentRound = 1;
        gameRoundManager = GetComponent<GameRoundManager>();
    }

    public void StartGame()
    {
        //Ensure that the server is the one sending game state to client
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
        //Start game with current network time to ensure late joiners have the correct countdown timer
        gameStarted = true;
        StartCoroutine(gameRoundManager.PreGame(startTime));
    }

    [Command(ignoreAuthority = true)]
    public void CmdPlayerDied(NetworkIdentity netId, int team)
    {
        //Send to client(s) that a player died
        RpcPlayerDied(netId, team);
    }



    [ClientRpc]
    void RpcPlayerDied(NetworkIdentity netId, int team)
    {
        //Client(s) receive message that player died
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
        //If the current round is the final one then end the game and not run the rest of the code
        if (currentRound == maxRounds)
        {
            GameEnd();
            return;
        }
        //Setup game for next round and update UI
        RoundEnd();
        blueTeamScore.text = "" + teamOneRoundsWon;
        redTeamScore.text = "" + teamTwoRoundsWon;
    }

    //When a player joins a game they request the current game state from the server
    [Command(ignoreAuthority = true)]
    public void CmdGetCurrentGameState()
    {
        
        if (isServer)
        {
            RpcGetCurrentGameState(gameStarted, currentRound, startTime);
        }
    }

    //Send the curent game state back to the client(s)
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

    //Function to start the round called from GameRoundManager
    public void RoundStart()
    {
        if (isServer)
        {
            startTime = NetworkTime.time;
            RpcStartRound(currentRound, startTime);
        }
    }

    //Inform the client(s) that the round has started
    [ClientRpc]
    void RpcStartRound(int currentRound, double startTime)
    {
        StartCoroutine(gameRoundManager.Round(currentRound, startTime));
    }
    //Server end round
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
    //Server end game
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
   
    //Client(s) setup for the next round
    [ClientRpc]
    void RpcStartPreRound(double startTime)
    {
        gameStarted = true;
        StartCoroutine(gameRoundManager.PreGame(startTime));
    }

    //Inform client(s) that the game has finished
    [ClientRpc]
    void RpcFinishGame()
    {
        StartCoroutine(EndOfGameDisplay());
    }

    //Show players who one and then load the menu scene
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

    //Display UI
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
    //Start of round enable players so they can move
    public void EnablePlayers()
    {
        if (isServer)
        {
            playerOne.RpcEnablePlayer();
            playerTwo.RpcEnablePlayer();
        }

    }

    //End of round disable players so they can't move
    public void DisablePlayers()
    {
        if (isServer)
        {
            playerOne.RpcDisablePlayer(playerOneSpawnPosition.position, playerOneSpawnPosition.rotation);
            playerTwo.RpcDisablePlayer(playerTwoSpawnPosition.position, playerTwoSpawnPosition.rotation);
        }
    }










}
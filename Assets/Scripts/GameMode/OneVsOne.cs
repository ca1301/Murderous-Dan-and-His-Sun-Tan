using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Mirror;

public class OneVsOne : NetworkBehaviour
{

    public static OneVsOne Instance = null;

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
    
    void Awake()
    {
        Instance = this;
        roundStatus.text = "Waiting for players";
        currentRound = 1;
    }

    private void Update()
    {
        if(isServer)
        {
           var players = FindObjectsOfType<NetworkPlayer>();
            if (players.Length >= 1 && !gameStarted)
            {
                StartGame();
            }

        }
    }


    void StartGame()
    {
        gameStarted = true;
        StartCoroutine(PreGame());
    }

    [Command(ignoreAuthority = true)]
    public void CmdGetCurrentGameState()
    {
        if (isServer)
        {
            RpcGetCurrentGameState(gameStarted, currentRound, roundGoing);
        }
    }

    [ClientRpc]
    public void RpcGetCurrentGameState(bool gameStarted, int currentRound, bool roundGoing)
    {
        if(!isServer)
        {
            this.gameStarted = gameStarted;
            this.currentRound = currentRound;
            this.roundGoing = roundGoing;
            if(gameStarted)
            {
                StartCoroutine(PreGame());
            }
        }
    }


    private IEnumerator PreGame()
    {
        float timer = OneVsOne_Settings.PRE_GAME_TIME;

        //Reset player one for next round


        while (timer > 0.0f)
        {
            double minutes = (int)timer / 60;
            double seconds = (int)timer % 60;
            roundStatus.text = "Game Starts In:";
            roundTime.text =  minutes.ToString() + " : " + seconds.ToString();
            yield return new WaitForEndOfFrame();

            timer -= Time.deltaTime;
        }
        //Count down timer finished
        roundGoing = true;
        StartCoroutine(Round(currentRound));

    }


    


    private IEnumerator Round(int round)
    {
        float timer = OneVsOne_Settings.ROUND_TIME;
        roundStatus.text = "Round: " + currentRound;
        while (timer > 0.0f && roundGoing)
        {

            double minutes = (int)timer / 60;
            double seconds = (int)timer % 60;
            roundTime.text = minutes.ToString() + " : " + seconds.ToString();

            yield return new WaitForEndOfFrame();
            timer -= Time.deltaTime;
        }

        if (currentRound >= OneVsOne_Settings.ROUNDS)
        {
            Debug.Log("Game Over");
            if (teamOneRoundsWon > teamTwoRoundsWon)
            {
                StartCoroutine(GameEnd(OneVsOne_Settings.TEAM_A));
                Debug.Log("Game Over1");
            }
            if (teamTwoRoundsWon > teamOneRoundsWon)
            {
                StartCoroutine(GameEnd(OneVsOne_Settings.TEAM_B));
                Debug.Log("Game Over2");
            }
        }
        else
        {
          
        }

    }

    private IEnumerator Intermission()
    {
        float timer = OneVsOne_Settings.INTERMISSION_TIME;

     



        while (timer > 0.0f)
        {
            double minutes = (int)timer / 60;
            double seconds = (int)timer % 60;
            roundTime.text = minutes.ToString() + " : " + seconds.ToString();
            roundStatus.text = "Intermission";
            yield return new WaitForEndOfFrame();

            timer -= Time.deltaTime;
        }

        roundGoing = true;
        //SetupPositions();
        StartCoroutine(Round(currentRound));

      
    }

    IEnumerator GameEnd(int winningTeam)
    {
        gameOverText.gameObject.SetActive(true);
        if (winningTeam == 0)
        {
            gameOverText.text = "Team One Won";
            gameOverText.color = Color.red;
        }
        else
        {
            gameOverText.text = "Team Two Won";
            gameOverText.color = Color.blue;
        }
        Cursor.lockState = CursorLockMode.None;
        yield return new WaitForSeconds(3);

    }

}
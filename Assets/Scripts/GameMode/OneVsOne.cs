using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Realtime;
using ExitGames.Client.Photon;
using Photon.Pun;
using UnityEngine.SceneManagement;
using TMPro;

public class OneVsOne : MonoBehaviourPunCallbacks
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

    // Start is called before the first frame update
    void Awake()
    {
        Instance = this;
        roundStatus.text = "Waiting for players";
        currentRound = 1;
    }

    public override void OnEnable()
    {
        PhotonNetwork.NetworkingClient.EventReceived += NetworkingClient_EventReceived;
    }
    public override void OnDisable()
    {
        PhotonNetwork.NetworkingClient.EventReceived -= NetworkingClient_EventReceived;
    }

    public override void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer)
    {
        CheckPlayersLoaded();
    }

    public override void OnJoinedRoom()
    {
        CheckPlayersLoaded();
    }



    void CheckPlayersLoaded()
    {
        Debug.Log(CheckAllPlayerLoadedLevel());
    }

    private void NetworkingClient_EventReceived(EventData obj)
    {

        if(obj.Code == OneVsOne_Settings.PLAYER_JOINED)
        {
            object[] info = (object[])obj.CustomData;
            if(playerOne == null)
            {
                PlayerJoinedTeam((string)info[0], 0);
                roundStatus.text = "Waiting for players 1/2";
            }
            else
            {
                PlayerJoinedTeam((string)info[0], 1);
                roundStatus.text = "Game about to start!!!";
                StartGame();
            }
        }

        if (obj.Code == OneVsOne_Settings.PLAYER_DIED)
        {
            object[] info = (object[])obj.CustomData;
            if((string)info[0] == playerOne.name)
            {
                EndRound(0);
            }
            else if((string)info[0] == playerTwo.name)
            {
                EndRound(1);
            }
            else if((string)info[0] == OneVsOne_Settings.TEAM_TIED)
            {
                EndRound(-1);
            }
            else
            {
                Debug.LogError("No Valid Team Found!!");
            }
        }
    }

    void StartGame()
    {
        gameStarted = true;
        StartCoroutine(PreGame());
    }

    private bool CheckAllPlayerLoadedLevel()
    {
        foreach (Photon.Realtime.Player p in PhotonNetwork.PlayerList)
        {
            object playerLoadedLevel;

            if (p.CustomProperties.TryGetValue(OneVsOne_Settings.PLAYER_LOADED_LEVEL, out playerLoadedLevel))
            {
                if ((bool)playerLoadedLevel)
                {
                    continue;
                }
            }

            return false;
        }

        return true;
    }



    private IEnumerator PreGame()
    {
        float timer = OneVsOne_Settings.PRE_GAME_TIME;

        //Reset player one for next round
        object[] playerOneObj = new object[] { playerOne.name, playerOneSpawnPosition.transform.position, playerOneSpawnPosition.transform.rotation};
        RaiseEventOptions raiseEventOptionsPlayerOne = new RaiseEventOptions { CachingOption = EventCaching.AddToRoomCache, Receivers = ReceiverGroup.All };
        SendOptions sendOptionsPlayerOne = new SendOptions { Reliability = true };
        PhotonNetwork.RaiseEvent(OneVsOne_Settings.PLAYER_ROUND_RESET, playerOneObj, raiseEventOptionsPlayerOne, sendOptionsPlayerOne);

        //Reset player one UI for next round
        object[] playerOneUI = new object[] { playerOne.name, 0 };
        RaiseEventOptions raiseEventOptionsPlayerOneUI = new RaiseEventOptions { CachingOption = EventCaching.AddToRoomCache, Receivers = ReceiverGroup.All };
        SendOptions sendOptionsPlayerOneUI = new SendOptions { Reliability = true };
        PhotonNetwork.RaiseEvent(OneVsOne_Settings.PLAYER_SET_UI, playerOneUI, raiseEventOptionsPlayerOneUI, sendOptionsPlayerOneUI);

        //Reset player two for next round
        object[] playerTwoObj = new object[] { playerTwo.name, playerTwoSpawnPosition.transform.position, playerTwoSpawnPosition.transform.rotation };
        RaiseEventOptions raiseEventOptionsPlayerTwo = new RaiseEventOptions { CachingOption = EventCaching.AddToRoomCache, Receivers = ReceiverGroup.All };
        SendOptions sendOptionsPlayerTwo = new SendOptions { Reliability = true };
        PhotonNetwork.RaiseEvent(OneVsOne_Settings.PLAYER_ROUND_RESET, playerTwoObj, raiseEventOptionsPlayerTwo, sendOptionsPlayerTwo);

        //Reset player two UI for next round
        object[] playerTwoUI = new object[] { playerTwo.name, 1 };
        RaiseEventOptions raiseEventOptionsPlayerTwoUI = new RaiseEventOptions { CachingOption = EventCaching.AddToRoomCache, Receivers = ReceiverGroup.All };
        SendOptions sendOptionsPlayerTwoUI = new SendOptions { Reliability = true };
        PhotonNetwork.RaiseEvent(OneVsOne_Settings.PLAYER_SET_UI, playerTwoUI, raiseEventOptionsPlayerTwoUI, sendOptionsPlayerTwoUI);




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


        object[] playerOneObject = new object[] { playerOne.name};
        RaiseEventOptions playerOneOptions = new RaiseEventOptions { CachingOption = EventCaching.AddToRoomCache, Receivers = ReceiverGroup.All };
        SendOptions playerOneSend = new SendOptions { Reliability = true };
        PhotonNetwork.RaiseEvent(OneVsOne_Settings.ROUND_READY, playerOneObject, playerOneOptions, playerOneSend);

        object[] playerTwoObject = new object[] { playerTwo.name };
        RaiseEventOptions playerTwoOptions = new RaiseEventOptions { CachingOption = EventCaching.AddToRoomCache, Receivers = ReceiverGroup.All };
        SendOptions playerTwoSend = new SendOptions { Reliability = true };
        PhotonNetwork.RaiseEvent(OneVsOne_Settings.ROUND_READY, playerTwoObject, playerTwoOptions, playerTwoSend);
    }


    public void PlayerJoinedTeam(string playerName, int team)
    {
        if (team == 0)
        {
            playerOne = GameObject.Find(playerName);
            object[] playerOneObj = new object[] { playerName, team };
            RaiseEventOptions raiseEventOptionsPlayerOne = new RaiseEventOptions { CachingOption = EventCaching.AddToRoomCache, Receivers = ReceiverGroup.All };
            SendOptions sendOptionsPlayerOne = new SendOptions { Reliability = true };
            PhotonNetwork.RaiseEvent(OneVsOne_Settings.PLAYER_JOIN_TEAM, playerOneObj, raiseEventOptionsPlayerOne, sendOptionsPlayerOne);
        }
        else
        {
            playerTwo = GameObject.Find(playerName);
            object[] playerOneObj = new object[] { playerName, team };
            RaiseEventOptions raiseEventOptionsPlayerOne = new RaiseEventOptions { CachingOption = EventCaching.AddToRoomCache, Receivers = ReceiverGroup.All };
            SendOptions sendOptionsPlayerOne = new SendOptions { Reliability = true };
            PhotonNetwork.RaiseEvent(OneVsOne_Settings.PLAYER_JOIN_TEAM, playerOneObj, raiseEventOptionsPlayerOne, sendOptionsPlayerOne);
        }
    }


    public void EndRound(int winningTeam)
    {
        //Check all win cases for the end of the round
        if(winningTeam == 0)
        {
            teamTwoRoundsWon += 1;
            blueTeamScore.text = teamTwoRoundsWon.ToString();
            StopAllCoroutines();
            roundGoing = false;
            currentRound += 1;
            StartCoroutine(Intermission());
        }
        else if(winningTeam == 1)
        {
            teamOneRoundsWon += 1;
            redTeamScore.text = teamOneRoundsWon.ToString();
            StopAllCoroutines();
            roundGoing = false;
            currentRound += 1;
            StartCoroutine(Intermission());
            
        }
        else if (winningTeam == -1)
        {
            Debug.Log("Draw!!");
            StopAllCoroutines();
            roundGoing = false;
            currentRound += 1;
            StartCoroutine(Intermission());
        }
    }

    private IEnumerator Round(int round)
    {
        float timer = OneVsOne_Settings.ROUND_TIME;
        // swatPlayersAlive = swat.Count;
        // terroristPlayersAlive = terrorists.Count;
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
            object[] content = new object[] { OneVsOne_Settings.TEAM_TIED };
            RaiseEventOptions raiseEventOptions = new RaiseEventOptions { CachingOption = EventCaching.AddToRoomCache, Receivers = ReceiverGroup.All };
            SendOptions sendOptions = new SendOptions { Reliability = true };
            PhotonNetwork.RaiseEvent(OneVsOne_Settings.PLAYER_DIED, content, raiseEventOptions, sendOptions);
        }

    }

    private IEnumerator Intermission()
    {
        float timer = OneVsOne_Settings.INTERMISSION_TIME;

        object[] playerOneObj = new object[] { playerOne.name, playerOneSpawnPosition.transform.position, playerOneSpawnPosition.transform.rotation };
        RaiseEventOptions raiseEventOptionsPlayerOne = new RaiseEventOptions { CachingOption = EventCaching.AddToRoomCache, Receivers = ReceiverGroup.All };
        SendOptions sendOptionsPlayerOne = new SendOptions { Reliability = true };
        PhotonNetwork.RaiseEvent(OneVsOne_Settings.PLAYER_ROUND_RESET, playerOneObj, raiseEventOptionsPlayerOne, sendOptionsPlayerOne);


        object[] playerTwoObj = new object[] { playerTwo.name, playerTwoSpawnPosition.transform.position, playerTwoSpawnPosition.transform.rotation };
        RaiseEventOptions raiseEventOptionsPlayerTwo = new RaiseEventOptions { CachingOption = EventCaching.AddToRoomCache, Receivers = ReceiverGroup.All };
        SendOptions sendOptionsPlayerTwo = new SendOptions { Reliability = true };
        PhotonNetwork.RaiseEvent(OneVsOne_Settings.PLAYER_ROUND_RESET, playerTwoObj, raiseEventOptionsPlayerTwo, sendOptionsPlayerTwo);



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

        object[] playerOneObject = new object[] { playerOne.name };
        RaiseEventOptions playerOneEvent = new RaiseEventOptions { CachingOption = EventCaching.AddToRoomCache, Receivers = ReceiverGroup.All };
        SendOptions playerOneSend = new SendOptions { Reliability = true };
        PhotonNetwork.RaiseEvent(OneVsOne_Settings.ROUND_READY, playerOneObject, playerOneEvent, playerOneSend);

        object[] playerTwoObject = new object[] { playerTwo.name };
        RaiseEventOptions playerTwoEvent = new RaiseEventOptions { CachingOption = EventCaching.AddToRoomCache, Receivers = ReceiverGroup.All };
        SendOptions playerTwoSend = new SendOptions { Reliability = true };
        PhotonNetwork.RaiseEvent(OneVsOne_Settings.ROUND_READY, playerTwoObject, playerTwoEvent, playerTwoSend);

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
        PhotonNetwork.LoadLevel(0);
    }

}
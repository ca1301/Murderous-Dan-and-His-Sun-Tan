using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class GameNetworkManager : NetworkManager
{
    [SerializeField]
    private Transform playerOneSpawn;
    [SerializeField]
    private Transform playerTwoSpawn;
    [SerializeField]
    private int minPlayersToStartGame = 2;

    //Spawn new player in different position depending on player count
    public override void OnServerAddPlayer(NetworkConnection conn)
    {
        Transform start = null;
        var players = FindObjectsOfType<NetworkPlayer>();
        if (players.Length == 0)
        {
            start = playerOneSpawn;
        }
        else
        {
            start = playerTwoSpawn;
        }

        GameObject player = Instantiate(playerPrefab, start.position, start.rotation);
        NetworkServer.AddPlayerForConnection(conn, player);

        if(numPlayers == minPlayersToStartGame)
        {
            GameManager.Instance.StartGame();
        }
    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Steamworks;
using TMPro;
public class LobbyButton : MonoBehaviour
{
    public ulong lobbyID;
    public string lobbyName;
    public TMP_Text buttonText;
    private void Start()
    {
        ulong steamID;
        ulong.TryParse(lobbyName, out steamID);
        lobbyName = SteamFriends.GetFriendPersonaName((CSteamID)steamID);
        buttonText.text = lobbyName;
    }

    public void JoinServer()
    {
        SteamMatchmaking.JoinLobby((CSteamID)lobbyID);
    }
}

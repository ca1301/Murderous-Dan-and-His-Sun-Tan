using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Steamworks;
using Mirror;
using TMPro;
public class SteamLobby : MonoBehaviour
{
    protected Callback<LobbyCreated_t> lobbyCreatedCallback;
    protected Callback<GameLobbyJoinRequested_t> lobbyJoinRequestedCallback;
    protected Callback<LobbyEnter_t> lobbyEnteredCallback;

    public List<CSteamID> lobbies = new List<CSteamID>();
    protected Callback<LobbyMatchList_t> lobbyMatchListCallback;
    protected Callback<LobbyDataUpdate_t> lobbyDataUpdateCallback;


    public TMP_Dropdown dropDown;
    public ELobbyType lobbyType;

    public List<GameObject> joinLobbyButtons = new List<GameObject>();
    public GameObject lobbyButton;
    public Transform lobbyButtonParent;

    private GameNetworkManager networkManager;

    private const string HostAddressKey = "HostAddress";

    public CSteamID currentLobby;
    // Start is called before the first frame update
    void Start()
    {
        networkManager = GetComponent<GameNetworkManager>();
        if (!SteamManager.Initialized)
            return;

        lobbyCreatedCallback = Callback<LobbyCreated_t>.Create(OnLobbyCreated);
        lobbyJoinRequestedCallback = Callback<GameLobbyJoinRequested_t>.Create(OnLobbyJoinRequested);
        lobbyEnteredCallback = Callback<LobbyEnter_t>.Create(OnLobbyEntered);

        lobbyMatchListCallback = Callback<LobbyMatchList_t>.Create(OnLobbyMatchList);
        lobbyDataUpdateCallback = Callback<LobbyDataUpdate_t>.Create(OnLobbyDataUpdate);
    }

    public void HostLobby()
    {
        lobbyType = (ELobbyType)dropDown.value;
        SteamMatchmaking.CreateLobby(lobbyType, networkManager.maxConnections);
    }


    private void OnLobbyCreated(LobbyCreated_t callback)
    {
        if(callback.m_eResult != EResult.k_EResultOK)
        {
            return;
        }
        networkManager.StartHost();

        SteamMatchmaking.SetLobbyData(new CSteamID(callback.m_ulSteamIDLobby), HostAddressKey, SteamUser.GetSteamID().ToString());
    }

    public void GetLobbies()
    {
        if(lobbies.Count > 0)
        {
            lobbies.Clear();
        }
        SteamMatchmaking.AddRequestLobbyListResultCountFilter(15);
        SteamMatchmaking.RequestLobbyList();
    }

    private void OnLobbyMatchList(LobbyMatchList_t callback)
    {
        lobbies.Clear();
        foreach (var item in joinLobbyButtons)
        {
            Destroy(item);
        }
        joinLobbyButtons.Clear();

        for (int i = 0; i < callback.m_nLobbiesMatching; i++)
        {
            CSteamID lobbyID = SteamMatchmaking.GetLobbyByIndex(i);
            lobbies.Add(lobbyID);
            SteamMatchmaking.RequestLobbyData(lobbyID);
            var lButton = Instantiate(lobbyButton, lobbyButtonParent.position, lobbyButtonParent.rotation);
            lButton.transform.SetParent(lobbyButtonParent, false);
            lButton.GetComponent<LobbyButton>().lobbyID = lobbyID.m_SteamID;
            lButton.GetComponent <LobbyButton>().lobbyName = SteamMatchmaking.GetLobbyData(new CSteamID(lobbyID.m_SteamID), HostAddressKey);
            joinLobbyButtons.Add(lButton);
        }
    }

    public void JoinLobby(ulong lobbyID)
    {
        SteamMatchmaking.JoinLobby((CSteamID)lobbyID);
    }

    private void OnLobbyDataUpdate(LobbyDataUpdate_t callback)
    {
        Debug.Log(callback.m_bSuccess);
    }


    private void OnLobbyJoinRequested(GameLobbyJoinRequested_t callback)
    {
        SteamMatchmaking.JoinLobby(callback.m_steamIDLobby);
    }

    private void OnLobbyEntered(LobbyEnter_t callback)
    {
        currentLobby = new CSteamID(callback.m_ulSteamIDLobby);
        if (NetworkServer.active) { return; }

        string hostAddress = SteamMatchmaking.GetLobbyData(new CSteamID(callback.m_ulSteamIDLobby), HostAddressKey);
        networkManager.networkAddress = hostAddress;
        networkManager.StartClient();
       
    }

}

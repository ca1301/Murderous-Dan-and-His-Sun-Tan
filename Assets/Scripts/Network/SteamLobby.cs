using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Steamworks;
using Mirror;
public class SteamLobby : MonoBehaviour
{
    protected Callback<LobbyCreated_t> lobbyCreatedCallback;
    protected Callback<GameLobbyJoinRequested_t> lobbyJoinRequestedCallback;
    protected Callback<LobbyEnter_t> lobbyEnteredCallback;
    public GameObject canvasToDisable;
    public GameObject canvasToEnable;
    public Camera cam;
    public ELobbyType lobbyType;
    private GameNetworkManager networkManager;

    private const string HostAddressKey = "HostAddress";
    // Start is called before the first frame update
    void Start()
    {
        networkManager = GetComponent<GameNetworkManager>();
        if (!SteamManager.Initialized)
            return;

        lobbyCreatedCallback = Callback<LobbyCreated_t>.Create(OnLobbyCreated);
        lobbyJoinRequestedCallback = Callback<GameLobbyJoinRequested_t>.Create(OnLobbyJoinRequested);
        lobbyEnteredCallback = Callback<LobbyEnter_t>.Create(OnLobbyEntered);
    }

    public void HostLobby()
    {
        canvasToDisable.SetActive(false);
        SteamMatchmaking.CreateLobby(lobbyType, networkManager.maxConnections);
    }

    private void OnLobbyCreated(LobbyCreated_t callback)
    {
        if(callback.m_eResult != EResult.k_EResultOK)
        {
            canvasToDisable.SetActive(true);
            return;
        }
        networkManager.StartHost();

        SteamMatchmaking.SetLobbyData(new CSteamID(callback.m_ulSteamIDLobby), HostAddressKey, SteamUser.GetSteamID().ToString());
    }

    private void OnLobbyJoinRequested(GameLobbyJoinRequested_t callback)
    {
        SteamMatchmaking.JoinLobby(callback.m_steamIDLobby);
    }

    private void OnLobbyEntered(LobbyEnter_t callback)
    {
        canvasToEnable.SetActive(true);
        cam.gameObject.SetActive(false);
        if (NetworkServer.active) { return; }

        string hostAddress = SteamMatchmaking.GetLobbyData(new CSteamID(callback.m_ulSteamIDLobby), HostAddressKey);
        networkManager.networkAddress = hostAddress;
        networkManager.StartClient();
        canvasToDisable.SetActive(false);
    }

}

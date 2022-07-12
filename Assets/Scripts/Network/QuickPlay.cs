using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;
using Steamworks;
using TMPro;
using UnityEngine.UI;
public class QuickPlay : MonoBehaviourPunCallbacks
{
    public Slider slider;
    public TMP_Text sensitivity;
    private void Start()
    {
        
        if (PlayerPrefs.GetFloat("Sens") == 0)
        {
            PlayerPrefs.SetFloat("Sens", 0.5f);
        }

        sensitivity.text = "" + PlayerPrefs.GetFloat("Sens");
        slider.value = PlayerPrefs.GetFloat("Sens");

        if (!SteamManager.Initialized)
        {
            Debug.LogError("Steam not initialized");
        }
        Screen.SetResolution(1920, 1080, FullScreenMode.Windowed);
        PhotonNetwork.ConnectUsingSettings();
    }

    public void SetValues()
    {
        PlayerPrefs.SetFloat("Sens", slider.value);
    }
    private void Update()
    {
        sensitivity.text = "" + slider.value.ToString(".0");
    }

    public override void OnConnectedToMaster()
    {
        if(SteamManager.Initialized)
        {
            //PhotonNetwork.LocalPlayer.NickName = SteamFriends.GetPersonaName();
            PhotonNetwork.LocalPlayer.NickName = "Player " + Random.Range(0, 1000);
        }
        else
        {
            PhotonNetwork.LocalPlayer.NickName = "Player " + Random.Range(0, 1000);
        }
       
    }

    public void QuickGame()
    {
        PhotonNetwork.JoinRandomRoom();
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        print("Failed to Join Random Room!!");
        CreateRandomRoom();
    }

    void CreateRandomRoom()
    {
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.IsVisible = true;
        roomOptions.MaxPlayers = 2;
        PhotonNetwork.CreateRoom("Room" + Random.Range(0, 1000), roomOptions, TypedLobby.Default);

    }

    public override void OnJoinedRoom()
    {
        Hashtable props = new Hashtable
            {
                {OneVsOne_Settings.PLAYER_LOADED_LEVEL, false}
            };
        PhotonNetwork.LocalPlayer.SetCustomProperties(props);
        print("Joined Room " + PhotonNetwork.CurrentRoom);
        PhotonNetwork.LoadLevel(1);
    }





}
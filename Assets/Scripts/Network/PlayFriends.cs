using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;
using UnityEngine.UI;

public class PlayFriends : MonoBehaviourPunCallbacks
{
    public Text name;


    public Text friendName;

    public string[] friends;
    private void Start()
    {
        Screen.SetResolution(1920, 1080, FullScreenMode.Windowed);
        AuthenticationValues authenticationValues = new AuthenticationValues();
        PhotonNetwork.AuthValues = authenticationValues;
        PhotonNetwork.SendRate = 20;

        PhotonNetwork.ConnectUsingSettings();
     
    }

    public override void OnConnectedToMaster()
    {
        print("Connected!!");
        //QuickGame();
    }

    public void SetPlayerName()
    {
        PhotonNetwork.LocalPlayer.NickName = name.text;
        PhotonNetwork.AuthValues.UserId = name.text;
    }

    public void FindFriends()
    {
        friends[0] = friendName.text;
        PhotonNetwork.FindFriends(friends );
    }
    public override void OnFriendListUpdate(List<FriendInfo> friendList)
    {
        base.OnFriendListUpdate(friendList);
        foreach (FriendInfo item in friendList)
        {
            Debug.Log("Friend Name: " + item.Name + " is in room: " + item.IsInRoom + " room name: " + item.Room);
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
        roomOptions.PublishUserId = true;
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
        //PhotonNetwork.LoadLevel(1);
    }





}
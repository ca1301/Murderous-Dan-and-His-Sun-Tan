using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class OneVsOne_Settings
{

    public const float PLAYER_RESPAWN_TIME = 4;

    public const int PLAYER_MAX_LIVES = 3;
    public const int ROUNDS = 5;


    public const float ROUND_TIME = 180;
    public const float INTERMISSION_TIME = 5;
    public const float PRE_GAME_TIME = 3;

    public const string PLAYER_LIVES = "PlayerLives";
    public const string PLAYER_READY = "IsPlayerReady";
    public const string PLAYER_LOADED_LEVEL = "PlayerLoadedLevel";
    public const string ROUND = "Round";
    public const string ROOM_SIZE = "RoomSize";


    public const string TEAM = "Team";
    public const int MIN_PLAYERS_PER_TEAM = 1;
    public const int TEAM_A = 0;
    public const int TEAM_B = 1;
    public const string TEAM_TIED = "Draw";

    public static string[] LEVELS = { "Arena1" };
    public const string LEVEL = "Level";




    //Event Codes
    public const byte GAME_START = 112;
    public const byte ROUND_READY = 118;
    public const byte ROUND_FINISHED = 113;
    public const byte PLAYER_DIED = 114;
    public const byte PLAYER_JOINED = 115;
    public const byte PLAYER_LEFT = 116;
    public const byte PLAYER_JOIN_TEAM = 117;
    public const byte PLAYER_ROUND_RESET = 121;
    public const byte PLAYER_SET_UI = 122;



}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
public class GameRoundManager : MonoBehaviour
{

    [Tooltip("Intermission time (in seconds)")]
    [SerializeField]
    private float intermissionTime = 2;

    [Tooltip("Round time (in seconds)")]
    [SerializeField]
    private float roundTime = 1000;





    public IEnumerator PreGame(double roundStartTime)
    {
        GameManager.Instance.DisablePlayers();
        float timer = intermissionTime;
        var difference = NetworkTime.time - roundStartTime;
        timer = timer - (float)difference;
        GameManager.Instance.roundStatus.text = "Round about to start";
        while (timer > 0.0f)
        {
            double minutes = (int)timer / 60;
            double seconds = (int)timer % 60;
            GameManager.Instance.roundTime.text = minutes.ToString() + " : " + seconds.ToString();
            yield return new WaitForEndOfFrame();

            timer -= Time.deltaTime;
        }
        GameManager.Instance.RoundStart();
    }

    public IEnumerator Round(int round, double roundStartTime)
    {
        GameManager.Instance.EnablePlayers();
        float timer = roundTime;
        var difference = NetworkTime.time - roundStartTime;
        timer = timer - (float)difference;
        GameManager.Instance.roundStatus.text = "Round: " + round;
        while (timer > 0.0f)
        {
            double minutes = (int)timer / 60;
            double seconds = (int)timer % 60;
            GameManager.Instance.roundTime.text = minutes.ToString() + " : " + seconds.ToString();
            yield return new WaitForEndOfFrame();
            timer -= Time.deltaTime;
        }
        GameManager.Instance.RoundEnd();
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
public class GameRoundManager : MonoBehaviour
{
    public int currentRound;
    GameManager gameManager;

    private void Start()
    {
        gameManager = GetComponent<GameManager>();
    }
    public IEnumerator PreGame(double roundStartTime)
    {
        float timer = 5;
        var difference = NetworkTime.time - roundStartTime;
        timer = timer - (float)difference;
        gameManager.roundStatus.text = "Round about to start";
        while (timer > 0.0f)
        {
            double minutes = (int)timer / 60;
            double seconds = (int)timer % 60;
            gameManager.roundTime.text = minutes.ToString() + " : " + seconds.ToString();
            yield return new WaitForEndOfFrame();

            timer -= Time.deltaTime;
        }

        gameManager.RoundStart();
    }

    public IEnumerator Round(int round, double roundStartTime)
    {
        float timer = 30;
        var difference = NetworkTime.time - roundStartTime;
        timer = timer - (float)difference;
        gameManager.roundStatus.text = "Round: " + round;
        while (timer > 0.0f)
        {
            double minutes = (int)timer / 60;
            double seconds = (int)timer % 60;
            gameManager.roundTime.text = minutes.ToString() + " : " + seconds.ToString();
            yield return new WaitForEndOfFrame();
            timer -= Time.deltaTime;
        }
        gameManager.RoundEnd();
    }
}

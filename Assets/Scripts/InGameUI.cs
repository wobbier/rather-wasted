using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InGameUI : MonoBehaviour
{
    struct PlayerView
    {
        public Text ScoreView;

        public void Refresh(GameState.PlayerState playerState)
        {
            ScoreView.text = playerState.Score.ToString();
            ScoreView.GetComponent<Animator>().Play("ScoreUpAnim");
        }
    }

    #region Public Members
    #endregion

    #region Private Members
    private GameState GState;

    private Canvas GameCanvas;
    private Text TimerText;

    private PlayerView[] PlayerViews = new PlayerView[2];
    #endregion

    // Use this for initialization
    void Start()
    {
        GState = FindObjectOfType<GameState>();
        GameCanvas = GetComponent<Canvas>();

        TimerText = GameObject.Find("GameUI/TimeText").GetComponent<Text>();

        PlayerViews[0].ScoreView = GameObject.Find("GameUI/Player1_Frame/Player1_Score").GetComponent<Text>();
        PlayerViews[1].ScoreView = GameObject.Find("GameUI/Player2_Frame/Player2_Score").GetComponent<Text>();

        PlayerViews[0].ScoreView.text = GState.GetPlayerState(0).Score.ToString();
        PlayerViews[1].ScoreView.text = GState.GetPlayerState(1).Score.ToString();

        GState.OnPlayerStateChanged += OnPlayerStateChanged;
    }

    private void OnPlayerStateChanged(int index)
    {
        GameState.PlayerState playerState = GState.GetPlayerState(index);
        PlayerViews[index].Refresh(playerState);
        
    }

    // Update is called once per frame
    void Update()
    {
        if (GState.CurrentState != GameState.EGameState.Ending)
        {
            TimeSpan timeRemaining = GState.GetRemaningTime();

            int minutes = timeRemaining.Minutes;
            int seconds = timeRemaining.Seconds;

            string minutesString = minutes.ToString();
            string secondsString = seconds.ToString("D2");

            TimerText.text = minutesString + ":" + secondsString;
        }
        else
        {
            TimerText.text = "FINISHED";
        }
    }
}

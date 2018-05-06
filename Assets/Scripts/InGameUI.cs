using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InGameUI : MonoBehaviour
{
    #region Public Members
    #endregion

    #region Private Members
    private GameState GState;

    private Canvas GameCanvas;
    private Text TimerText;
    #endregion

    // Use this for initialization
    void Start()
    {
        GState = FindObjectOfType<GameState>();
        GameCanvas = GetComponent<Canvas>();

        TimerText = GameObject.Find("GameUI/TimeText").GetComponent<Text>();
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

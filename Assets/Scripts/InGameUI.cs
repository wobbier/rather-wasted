using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InGameUI : MonoBehaviour
{
    class PlayerView
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

    private List<PlayerView> PlayerViews = new List<PlayerView>();

    private PlayerView WinnerView;
    private PlayerView LoserView;
    #endregion

    // Use this for initialization
    void Start()
    {
        GState = FindObjectOfType<GameState>();
        GameCanvas = GetComponent<Canvas>();

        TimerText = GameObject.Find("GameUI/TimeText").GetComponent<Text>();

        PlayerViews.Add(new PlayerView());
        PlayerViews.Add(new PlayerView());

        PlayerViews[0].ScoreView = GameObject.Find("GameUI/Player1_Frame/Player1_Score").GetComponent<Text>();
        PlayerViews[1].ScoreView = GameObject.Find("GameUI/Player2_Frame/Player2_Score").GetComponent<Text>();

        PlayerViews[0].ScoreView.text = GState.GetPlayerState(0).Score.ToString();
        PlayerViews[1].ScoreView.text = GState.GetPlayerState(1).Score.ToString();

        GState.OnPlayerStateChanged += OnPlayerStateChanged;
        GState.OnGameEnd += OnGameEnd;
    }

    private void OnGameEnd()
    {
        TimerText.text = "FINISHED";

        int winnerIdx = GState.FindWinner();
        bool bIsTie = winnerIdx == -1;

        if (!bIsTie)
        {
            WinnerView = PlayerViews[winnerIdx];
            LoserView = PlayerViews.Find(view => view != WinnerView);
        }
        else
        {
            TimerText.text = "TIE!";
        }
    }

    private void OnPlayerStateChanged(int index)
    {
        GameState.PlayerState playerState = GState.GetPlayerState(index);
        PlayerViews[index].Refresh(playerState);
    }

    // Update is called once per frame
    void Update()
    {
        if (GState.CurrentState == GameState.EGameState.Beginning)
        {
            TimeSpan timeRemaining = GState.GetCountDownRemaining();

            int minutes = timeRemaining.Minutes;
            int seconds = timeRemaining.Seconds;

            string minutesString = minutes.ToString();
            string secondsString = seconds.ToString("D2");

            TimerText.text = minutesString + ":" + secondsString;
        }
        else if (GState.CurrentState == GameState.EGameState.Playing)
        {
            TimeSpan timeRemaining = GState.GetRemaningTime();

            int minutes = timeRemaining.Minutes;
            int seconds = timeRemaining.Seconds;

            string minutesString = minutes.ToString();
            string secondsString = seconds.ToString("D2");

            TimerText.text = minutesString + ":" + secondsString;
        }
        else if (GState.CurrentState == GameState.EGameState.Ending)
        {
            WinnerView.ScoreView.gameObject.transform.parent.position = Vector3.Lerp(WinnerView.ScoreView.gameObject.transform.parent.position, new Vector3(Screen.width / 2.0f, Screen.height / 2.0f), 2.0f * Time.deltaTime);
            WinnerView.ScoreView.gameObject.transform.parent.localScale = Vector3.Lerp(WinnerView.ScoreView.gameObject.transform.parent.localScale, new Vector3(2.0f, 2.0f), 2.0f * Time.deltaTime);

            Vector3 screenMid = new Vector3(Screen.width / 2.0f, Screen.height / 2.0f);
            Vector3 pos = WinnerView.ScoreView.gameObject.transform.parent.position;

            int xMultiplier = -1;
            if ((screenMid - pos).x < 0)
            {
                xMultiplier = 1;
            }

            int fontSize = WinnerView.ScoreView.fontSize;
            fontSize = (int)Mathf.Lerp(fontSize, 32.0f, 2.0f);
            WinnerView.ScoreView.fontSize = fontSize;

            LoserView.ScoreView.gameObject.transform.parent.position = Vector3.Lerp(LoserView.ScoreView.gameObject.transform.parent.position, new Vector3((Screen.width / 2.0f) - (200.0f * xMultiplier), (Screen.height / 2.0f) + 100.0f), 2.0f * Time.deltaTime);
            LoserView.ScoreView.gameObject.transform.parent.localScale = Vector3.Lerp(LoserView.ScoreView.gameObject.transform.parent.localScale, new Vector3(0.75f, 0.75f), 2.0f * Time.deltaTime);
        }
    }
}

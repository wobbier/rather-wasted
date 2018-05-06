using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameState : MonoBehaviour
{
    public enum EGameState
    {
        Playing,
        Ending
    }

    [System.Serializable]
    public struct GameDuration
    {
        public int minutes;
        public int seconds;
    }

    #region Private Members
    private DateTime EndTime;
    #endregion

    #region Public Members
    public GameDuration PlayDuration;

    public EGameState CurrentState;
    #endregion

    // Use this for initialization
    void Start()
    {
        EndTime = DateTime.Now.AddMinutes(PlayDuration.minutes).AddSeconds(PlayDuration.seconds);
    }

    // Update is called once per frame
    void Update()
    {
        TimeSpan remaining = GetRemaningTime();
        if (remaining.TotalSeconds <= 0)
        {
            CurrentState = EGameState.Ending;
        }
    }

    #region Public Methods
    public TimeSpan GetRemaningTime()
    {
        return EndTime - DateTime.Now;
    }
    #endregion
}

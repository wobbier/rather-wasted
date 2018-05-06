using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameState : MonoBehaviour
{
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
        Debug.Log(remaining);
    }

    #region Public Methods
    public TimeSpan GetRemaningTime()
    {
        return EndTime - DateTime.Now;
    }
    #endregion
}

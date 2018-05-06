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

    public struct PlayerState
    {
        public PlayerController Controller;
        public int Score;
    }

    #region Public Delegates
    public delegate void PlayerStateChangedDelegate(int index);

    public event PlayerStateChangedDelegate OnPlayerStateChanged;
    #endregion

    #region Private Members
    private DateTime EndTime;

    private PlayerState[] PlayerStates = new PlayerState[2];
    #endregion

    #region Public Members
    public GameDuration PlayDuration;

    public EGameState CurrentState;
    #endregion

    // Use this for initialization
    void Start()
    {
        EndTime = DateTime.Now.AddMinutes(PlayDuration.minutes).AddSeconds(PlayDuration.seconds);

        PlayerStates[0].Controller = FindObjectsOfType<PlayerController>()[1];
        PlayerStates[1].Controller = FindObjectsOfType<PlayerController>()[0];

        PlayerStates[0].Controller.OnAttackSuccess += Player1_AttackSuccess;
        PlayerStates[1].Controller.OnAttackSuccess += Player2_AttackSuccess;
    }

    private void Player2_AttackSuccess()
    {
        PlayerStates[1].Score++;

        if (OnPlayerStateChanged != null)
        {
            OnPlayerStateChanged.Invoke(1);
        }
    }

    private void Player1_AttackSuccess()
    {
        PlayerStates[0].Score++;

        if (OnPlayerStateChanged != null)
        {
            OnPlayerStateChanged.Invoke(0);
        }
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

    public PlayerState GetPlayerState(int index)
    {
        if (index > 1)
        {
            Debug.LogError("Index greater than allowed values: " + index);
            return new PlayerState();
        }

        return PlayerStates[index];
    }
    #endregion
}

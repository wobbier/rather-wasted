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
    public delegate void GameFinishedDelegate();

    public event PlayerStateChangedDelegate OnPlayerStateChanged;
    public event GameFinishedDelegate OnGameEnd;
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
            EndGame();
        }
    }

    private void EndGame()
    {
        CurrentState = EGameState.Ending;

        foreach (PlayerState state in PlayerStates)
        {
            //state.Controller.ApplyMovement = false;
        }

        if (OnGameEnd != null)
        {
            OnGameEnd.Invoke();
        }
    }

    #region Public Methods
    public int FindWinner()
    {
        int winnerIdx = (PlayerStates[0].Score > PlayerStates[1].Score) ? 0 : 1;
        return (PlayerStates[0].Score == PlayerStates[1].Score) ? -1 : winnerIdx;
    }

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

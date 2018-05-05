using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SharkController : PlayerController
{
    private enum CharacterState
    {
        Idle,
        Moving,
        PrimaryAttack,
        SecondaryAttack,
        Recovery
    }

    private CharacterState m_state;
    private CharacterState m_previousState;

    public Animator SkinAnimator;

    private void Awake()
    {
        OnMovementStep += OnMove;
        SkinAnimator.Play("Idle");
        m_state = CharacterState.Idle;
    }

    private void OnMove(float val)
    {
        if(m_state != CharacterState.Moving)
        {
            SkinAnimator.Play("Walk");
            m_state = CharacterState.Moving;
        }
    }

    private void PrimaryAttack()
    {
        m_state = CharacterState.PrimaryAttack;
        SkinAnimator.Play("Slam");
    }

    public void Update()
    {
        if(Input.GetKeyDown(KeyCode.E))
        {
            PrimaryAttack();
        }

        if (m_state != m_previousState)
        {

        }
    }

}

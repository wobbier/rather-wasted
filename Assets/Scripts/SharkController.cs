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
        OnMovementBegin += OnMove;
        OnMovementEnd += OnStop;
        SkinAnimator.Play("Idle");
        m_state = CharacterState.Idle;
    }

    private void OnStop()
    {
        if (m_state != CharacterState.PrimaryAttack || m_state == CharacterState.Idle)
        {
            SkinAnimator.StopPlayback();
            SkinAnimator.Play("Idle");
            m_state = CharacterState.Idle;
        }
    }

    private void OnMove()
    {
        if (m_state == CharacterState.Idle)
        {
            SkinAnimator.Play("Walk");
            m_state = CharacterState.Moving;
        }
    }

    private void PrimaryAttack()
    {
        m_state = CharacterState.PrimaryAttack;
        SkinAnimator.Play("Slam");
        ApplyMovement = false;
    }

    public void LateUpdate()
    {
        if (m_state == CharacterState.PrimaryAttack)
        {
            if (SkinAnimator.GetCurrentAnimatorStateInfo(0).IsName("Idle"))
            {
                m_state = CharacterState.Idle;
                ApplyMovement = true;
            }
            return;
        }
        if(Input.GetKeyDown(KeyCode.E))
        {
            PrimaryAttack();
        }

        if (m_state != m_previousState)
        {

        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(m_state == CharacterState.PrimaryAttack && other.transform.tag == "Civillian")
        {
            //other.gameObject.GetComponent<Animator>().Play("Death");
            Destroy(other.gameObject, 1);
            other.gameObject.SetActive(false);
        }
    }

}

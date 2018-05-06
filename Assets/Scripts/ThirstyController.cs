using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThirstyController : PlayerController
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
        OnMainAbility += OnPrimaryAttack;
        SkinAnimator.Play("Idle");
        m_state = CharacterState.Idle;
    }

    private void OnPrimaryAttack()
    {
        m_state = CharacterState.PrimaryAttack;
        SkinAnimator.Play("Slam");
        ApplyMovement = false;
    }

    private void OnStop()
    {
        if (m_state != CharacterState.PrimaryAttack || m_state == CharacterState.Idle)
        {
            //SkinAnimator.StopPlayback();
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

    public void LateUpdate()
    {
        if (m_state == CharacterState.PrimaryAttack)
        {
            if (SkinAnimator.GetCurrentAnimatorStateInfo(0).IsName("SlamRecovery"))
            {
                m_state = CharacterState.Idle;
                SkinAnimator.Play("Idle");
                ApplyMovement = true;
            }
            return;
        }

        float velocity = GetComponent<Rigidbody>().velocity.sqrMagnitude;
        if (velocity > 0.2f)
        {
            //SkinAnimator.Play("Walk");
        }
        else
        {
            //SkinAnimator.Play("Idle");
        }

        if (m_state != m_previousState)
        {

        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (m_state == CharacterState.PrimaryAttack && other.transform.tag == "Civillian")
        {
            //other.gameObject.GetComponent<Animator>().Play("Death");
            Destroy(other.gameObject, 1);
            other.gameObject.SetActive(false);

            AttackSuccess();
        }
    }

}

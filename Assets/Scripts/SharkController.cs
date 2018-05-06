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
        SkinAnimator.Play("Idle");
        m_state = CharacterState.Idle;
    }

    private void OnMove()
    {
        if(m_state == CharacterState.Idle)
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

    public void LateUpdate()
    {
        if (m_state == CharacterState.PrimaryAttack)
        {
            if (SkinAnimator.GetCurrentAnimatorStateInfo(0).IsName("Idle"))
            {
                m_state = CharacterState.Idle;
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

    private void OnCollisionEnter(Collision collision)
    {
        if(m_state == CharacterState.PrimaryAttack && collision.transform.tag == "Civillian")
        {
            collision.gameObject.GetComponent<Animator>().Play("Death");
            Destroy(collision.gameObject, 1);
        }
    }

}

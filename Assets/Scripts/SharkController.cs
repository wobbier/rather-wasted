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

    private void OnTriggerEnter(Collider other)
    {
        if(m_state == CharacterState.PrimaryAttack && other.transform.tag == "Civillian")
        {
            //other.gameObject.GetComponent<Animator>().Play("Death");
            Destroy(other.gameObject, 1);
        }
    }

}

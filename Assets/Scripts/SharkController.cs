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

    public Transform Mouth;

    private void Awake()
    {
        OnMovementBegin += OnMove;
        OnMovementEnd += OnStop;
        OnMainAbility += OnPrimaryAttack;
        OnRotationBegin += OnTurn;
        SkinAnimator.Play("Idle");
        m_state = CharacterState.Idle;
    }

    private void OnTurn(float x)
    {
        if (x < 0.4f)
        {
            SkinAnimator.CrossFade("TurnLeft", Time.deltaTime * 5.0f);
        }
        else if (x > 0.4f)
        {
            SkinAnimator.CrossFade("TurnRight", Time.deltaTime * 5.0f);

        }
        else
        {
            SkinAnimator.CrossFade("Walk", Time.deltaTime * 5.0f);
        }
    }

    private void OnPrimaryAttack()
    {
        m_state = CharacterState.PrimaryAttack;
        SkinAnimator.Play("Bite");
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
            if (SkinAnimator.GetCurrentAnimatorStateInfo(0).IsName("Idle"))
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

    private void OnTriggerStay(Collider other)
    {
        if (m_state == CharacterState.PrimaryAttack && other.transform.tag == "Civilian" && !other.GetComponent<CivilianAI>().IsDead())
        {
            //other.gameObject.GetComponent<Animator>().Play("Death");
            other.transform.parent = Mouth;
            other.transform.localPosition = Vector3.zero;
            other.transform.rotation = Quaternion.LookRotation(Vector3.up, -Vector3.right);
            other.GetComponent<CivilianAI>().Die();
            Destroy(other.gameObject, 2);
            //other.gameObject.SetActive(false);
            AttackSuccess();
        }
    }

}

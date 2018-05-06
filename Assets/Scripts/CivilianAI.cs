using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CivilianAI : MonoBehaviour
{
    private NavMeshAgent m_nav;
    public float NavTimeout = 10;
    private float m_navTimeoutTimer = 0;

    private bool m_isDead = false;

    public bool IsDead() { return m_isDead; }

    public Animator SkinAnimator;
    // Use this for initialization
    void Start()
    {
        m_nav = GetComponent<NavMeshAgent>();
    }

    // Update is called once per frame
    void Update()
    {
        m_navTimeoutTimer -= Time.deltaTime;

        if (!m_nav.hasPath && m_navTimeoutTimer < 0)
        {
            m_nav.SetDestination(new Vector3(Random.Range(0, 20), 0, Random.Range(0, 200)));
            m_navTimeoutTimer = NavTimeout;
        }

        float velocity = m_nav.velocity.sqrMagnitude;
        if (velocity > 0.2f)
        {
            SkinAnimator.Play("run");
        }
        else
        {
            SkinAnimator.Play("idle");
        }
    }


    public void Die()
    {
        if(!m_isDead)
        {
            m_isDead = true;
            SkinAnimator.StopPlayback();
            SkinAnimator.Play("run");
            m_nav.isStopped = true;
            GetComponent<Collider>().enabled = false;
            m_nav.enabled = false;
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(SphereCollider))]
public class CivillianAI : MonoBehaviour
{
    private NavMeshAgent m_nav;
    public float NavTimeout = 10;
    private float m_navTimeoutTimer = 10;
    // Use this for initialization
    void Start()
    {
        m_nav = GetComponent<NavMeshAgent>();
    }

    // Update is called once per frame
    void Update()
    {
        m_navTimeoutTimer -= Time.deltaTime;

        if (!m_nav.hasPath || m_navTimeoutTimer < 0)
        {
            m_nav.SetDestination(new Vector3(Random.Range(0, 20), 0, Random.Range(0, 200)));
            m_navTimeoutTimer = NavTimeout;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {

        }
    }
}

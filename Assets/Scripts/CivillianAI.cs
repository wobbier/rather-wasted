using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(SphereCollider))]
public class CivillianAI : MonoBehaviour
{
    private NavMeshAgent m_nav;
    // Use this for initialization
    void Start()
    {
        m_nav = GetComponent<NavMeshAgent>();
    }

    // Update is called once per frame
    void Update()
    {
        if(!m_nav.hasPath)
        {
            m_nav.SetDestination(new Vector3(Random.Range(0, 20), 0, Random.Range(0, 20)));
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {

        }
    }
}

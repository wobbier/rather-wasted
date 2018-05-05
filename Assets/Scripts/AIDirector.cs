using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIDirector : MonoBehaviour {

    public int InitialAmountToSpawn = 5;

    public GameObject Civillian;

    private AISpawnPoint[] m_spawnPoints;

    // Use this for initialization
    void Awake()
    {
        m_spawnPoints = FindObjectsOfType<AISpawnPoint>();

        for (int i = 0; i < InitialAmountToSpawn; ++i)
        {
            Instantiate(Civillian, m_spawnPoints[Random.Range(0, m_spawnPoints.Length)].transform.position, Quaternion.identity);
        }

    }
	
	// Update is called once per frame
	void Update () {
		
	}
}

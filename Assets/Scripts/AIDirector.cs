﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIDirector : MonoBehaviour {

    public int InitialAmountToSpawn = 5;

    public GameObject Civilian;
    public float SpawnRate;

    private AISpawnPoint[] m_spawnPoints;
    private float m_spawnTimer = 5;

    // Use this for initialization
    void Awake()
    {
        m_spawnPoints = FindObjectsOfType<AISpawnPoint>();

        for (int i = 0; i < InitialAmountToSpawn; ++i)
        {
            Instantiate(Civilian, m_spawnPoints[Random.Range(0, m_spawnPoints.Length)].transform.position, Quaternion.identity);
        }
        m_spawnTimer = SpawnRate;
    }
	
	// Update is called once per frame
	void Update () {
        m_spawnTimer -= Time.deltaTime;
        if(m_spawnTimer < 0)
        {
            Instantiate(Civilian, m_spawnPoints[Random.Range(0, m_spawnPoints.Length)].transform.position, Quaternion.identity);
            m_spawnTimer = SpawnRate;
        }
	}
}

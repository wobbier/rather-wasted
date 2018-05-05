using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AISpawnPoint : MonoBehaviour {
    public Mesh GizmoMesh;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void OnDrawGizmos () {
        Gizmos.DrawCube(transform.position, new Vector3() * 10);
	}
}

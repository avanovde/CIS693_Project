using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResizeBox : MonoBehaviour {

	private float z = 0f;
	private float previousZ = 0f;
	private float count = 0;
	//private GameObject cube;
	// Use this for initialization
	void Start () {
		//cube = GetComponent<GameObject> ();
	}
	
	// Update is called once per frame
	void Update () {
		count = count + 1;
		z = 0.5f * Mathf.Sin (count / 20); 
		float deltaZ = z - previousZ;
		previousZ = z;
		transform.Translate (new Vector3 (deltaZ, deltaZ, deltaZ));
	}
}

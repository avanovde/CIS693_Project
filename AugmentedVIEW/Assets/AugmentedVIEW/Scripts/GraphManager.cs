using UnityEngine;
using System.Collections;

public class GraphManager : MonoBehaviour
{
	public Vector3 TargetPosition;
	public Quaternion TargetRotation;

	// Use this for initialization
	void Start ()
	{
	}
	
	// Update is called once per frame
	void Update ()
	{
		transform.position = TargetPosition;
		transform.rotation = TargetRotation;
	}
}


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
		transform.position = Vector3.Lerp (transform.position, TargetPosition, Time.deltaTime);
		transform.rotation = TargetRotation;
	}
}


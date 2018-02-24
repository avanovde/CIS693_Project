using UnityEngine;
using System.Collections;

public class GraphManager : MonoBehaviour
{
	public Vector3 TargetPosition;

	// Use this for initialization
	void Start ()
	{
	}
	
	// Update is called once per frame
	void Update ()
	{
		transform.position = Vector3.Lerp (transform.position, TargetPosition, Time.deltaTime);
	}
}


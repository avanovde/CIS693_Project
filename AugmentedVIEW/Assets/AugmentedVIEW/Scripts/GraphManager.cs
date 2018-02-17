using UnityEngine;
using System.Collections;

public class GraphManager : MonoBehaviour
{
	private ResizeBox _box;
	public Vector3 TargetPosition;

	void Awake()
	{
		_box = GetComponent<ResizeBox> ();
	}

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


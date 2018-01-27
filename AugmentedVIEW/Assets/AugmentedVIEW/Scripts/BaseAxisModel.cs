using UnityEngine;
using System.Collections;

public class BaseAxisModel : MonoBehaviour
{
	private enum AxisState
	{
		Locked = 0,
		Selected
	}
	public GameObject XLine, YLine, ZLine;
	public float DistanceFromCamera;
	private AxisState CurrentAxisState = AxisState.Selected;
	private Camera mainCamera;

	void Start()
	{
		mainCamera = Camera.main;
	}

	void Update()
	{
		switch (CurrentAxisState) 
		{
		case AxisState.Locked:
			break;
		case AxisState.Selected:
			// Update the location of the axis object
			transform.position = mainCamera.transform.position + mainCamera.transform.forward * DistanceFromCamera;
			transform.rotation = mainCamera.transform.rotation;
			break;
		default:
			Debug.Assert(false, "Unknown State");
			break;
		}
	}
}


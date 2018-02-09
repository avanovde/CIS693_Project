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
			var cameraTransform = mainCamera.transform;
			var cameraPosition = cameraTransform.position;

			transform.position = (cameraPosition + cameraTransform.forward * DistanceFromCamera);
			transform.position = (transform.position + transform.up * -.02f);
			transform.rotation = mainCamera.transform.rotation;
			break;
		default:
			Debug.Assert(false, "Unknown State");
			break;
		}
	}

	/// <summary>
	/// Toggles the locking state of a positioner.  That way it can follow the
	/// camera, or stay at a fixed location.
	/// </summary>
	public void ToggleState() {
		if (CurrentAxisState == AxisState.Selected) {
			CurrentAxisState = AxisState.Locked;
			Debug.Log ("Locking the graph positioner location");
			return;
		}
		if (CurrentAxisState == AxisState.Locked) {
			CurrentAxisState = AxisState.Selected;
			Debug.Log ("Unlocking the graph positioner location");
			return;
		}
	}
}


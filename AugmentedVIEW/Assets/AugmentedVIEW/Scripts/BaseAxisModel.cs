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
//		if (Input.GetMouseButtonDown(0)) {
//			Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
//			RaycastHit raycastHit;
//
//			Debug.DrawRay(ray.origin, ray.direction, Color.white, 1, false);
//			if (Physics.Raycast(ray, out raycastHit, 30f)) {
//				Debug.Log("User pressed the " + raycastHit.collider.gameObject.name + " object");
//				if (raycastHit.collider.gameObject.name == "GraphPositioner") {
//					Debug.Log("Placing graph positioner");

//
//				}
//			}
//		}


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


using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TouchController : MonoBehaviour {
	
	// Update is called once per frame
	void Update () {
		#if UNITY_EDITOR
		if (Input.GetMouseButtonDown(0)) {
			Ray raycast = Camera.main.ScreenPointToRay(Input.mousePosition);
			RaycastHit raycastHit;

			Debug.DrawRay(raycast.origin, raycast.direction, Color.white, 1, false);
			if (Physics.Raycast(raycast, out raycastHit)) {
				Debug.Log("User pressed the " + raycastHit.collider.name + " object");
			} else {
				Debug.Log("Raycast missed");
			}
		}
		#else
		if ((Input.touchCount > 0) && (Input.GetTouch (0).phase == TouchPhase.Began)) {
		Ray raycast = Camera.main.ScreenPointToRay (Input.GetTouch (0).position);
		RaycastHit raycastHit;
		if (Physics.Raycast(raycast, out raycastHit)) {
		Debug.Log("User pressed the " + raycastHit.collider.name + " object");
		}
		}
		#endif
	}
}

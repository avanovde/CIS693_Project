﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.iOS;

public class TouchController : MonoBehaviour {

	// Unity ARKit documentation
	public Transform m_HitTransform;
	public float maxRayDistance = 30.0f;
	public LayerMask collisionLayer = 1 << 10;  //ARKitPlane layer

	bool HitTestWithResultType (ARPoint point, ARHitTestResultType resultTypes)
	{
		List<ARHitTestResult> hitResults = UnityARSessionNativeInterface.GetARSessionNativeInterface ().HitTest (point, resultTypes);
		if (hitResults.Count > 0) {
			foreach (var hitResult in hitResults) {
				Debug.Log ("Got hit!");
				m_HitTransform.position = UnityARMatrixOps.GetPosition (hitResult.worldTransform);
				m_HitTransform.rotation = UnityARMatrixOps.GetRotation (hitResult.worldTransform);
				Debug.Log (string.Format ("x:{0:0.######} y:{1:0.######} z:{2:0.######}", m_HitTransform.position.x, m_HitTransform.position.y, m_HitTransform.position.z));
				return true;
			}
		}
		return false;
	}

	// Update is called once per frame
	void Update () {
		#if UNITY_EDITOR
		if (Input.GetMouseButtonDown(0)) {
			Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
			RaycastHit raycastHit;
			Debug.DrawRay(ray.origin, ray.direction, Color.yellow, 2.0f);
			if (Physics.Raycast(ray, out raycastHit, maxRayDistance)) {
				Debug.Log("User pressed the " + raycastHit.collider.gameObject.name + " object");
				if (raycastHit.collider.name == "GraphPositioner") {
					var positionerObject = raycastHit.collider.gameObject;
					BaseAxisModel positioner = positionerObject.GetComponent<BaseAxisModel> ();
					if (positioner == null) {
						Debug.Log ("No BaseAxisModel script on the positioner game object pressed");
						return;
					}

					positioner.ToggleState ();
				}
			}
		}
		#else
		if (Input.touchCount > 0 && m_HitTransform != null)
		{
			var touch = Input.GetTouch(0);
			if (touch.phase == TouchPhase.Began)
			{
				Ray ray = Camera.main.ScreenPointToRay (touch.position);
				RaycastHit raycastHit;
		Debug.DrawRay(ray.origin, ray.direction, Color.yellow, 2.0f);
				if (Physics.Raycast(ray, out raycastHit, maxRayDistance)) {
					Debug.Log("User pressed the " + raycastHit.collider.gameObject.name + " object");
					if (raycastHit.collider.name == "GraphPositioner") {
						var positionerObject = raycastHit.collider.gameObject;
						BaseAxisModel positioner = positionerObject.GetComponent<BaseAxisModel> ();
						if (positioner == null) {
							Debug.Log ("No BaseAxisModel script on the positioner game object pressed");
							return;
						}
	
						positioner.ToggleState ();
					}
				}
			}
		}
		#endif
	}
}

﻿using UnityEngine;
using System.Collections;
using UnityEngine.XR.iOS;
using System.Collections.Generic;

public class GraphPositioner : MonoBehaviour
{
	const int MOUSE_LEFT = 0;
	const int MOUSE_RIGHT = 1;
	const int MOUSE_MIDDLE = 2;

	public enum PositionerState {
		Initializing,
		FindingPlane,
		FoundPlane,
		Locked
	}
	private PositionerState _currentState;
	public PositionerState CurrentState {
		get {
			return _currentState;
		}
		set {
			_currentState = value; // Do other stuff here too
		}
	}

	// Game object for the positioner
	public GameObject findingPlaneObject;
	public GameObject foundPlaneObject;
	//TODO public GameObject lockedPlaneObject;

	// Touch and placement info for the positioner
	public float maxRayDistance = 30.0f;
	public float positionerDistance = 0.5f;
	public LayerMask collisionLayerMask;

	public GameObject GraphPrefab { get; set; }
	public IDataProvider DataProvider { get; set;}
		
	private GameObject _graph { get; set; }

	bool trackingInitialized;

	bool HitTestWithResultType (ARPoint point, ARHitTestResultType resultTypes)
	{
		List<ARHitTestResult> hitResults = UnityARSessionNativeInterface.GetARSessionNativeInterface ().HitTest (point, resultTypes);
		if (hitResults.Count > 0) {
			foreach (var hitResult in hitResults) {
				foundPlaneObject.transform.position = UnityARMatrixOps.GetPosition (hitResult.worldTransform);
				foundPlaneObject.transform.rotation = UnityARMatrixOps.GetRotation (hitResult.worldTransform);
				Debug.Log (string.Format ("x:{0:0.######} y:{1:0.######} z:{2:0.######}", foundPlaneObject.transform.position.x, foundPlaneObject.transform.position.y, foundPlaneObject.transform.position.z));
				return true;
			}
		}
		return false;
	}

	void Start()
	{
		CurrentState = PositionerState.Initializing;
		trackingInitialized = true;
	}

	void Update()
	{
		UpdatePositionerLocation ();
		CheckForUserPress ();
	}

	/// <summary>
	/// Checks to see if the user clicked on the graph positioner.  If they did,
	/// then the graph is positioned there, and we get rid of the positioner.
	/// </summary>
	private void CheckForUserPress()
	{
		#if UNITY_EDITOR
		if (Input.GetMouseButtonDown(MOUSE_LEFT))
		{
			var mousePosition = Input.mousePosition;
			RaycastHit hit;
			Ray userClickRay = Camera.main.ScreenPointToRay(mousePosition);
			if (Physics.Raycast(userClickRay, out hit)) 
			{
				ToggleState();
			}

		}
		#else


		#endif
	}

	/// <summary>
	/// Updates the positioner location based on where the center of the screen is pointing.  If
	/// it is not pointing at a horizontal plane, then the state is set to finding plane.
	/// </summary>
	private void UpdatePositionerLocation()
	{
		//use center of screen for focusing
		Vector3 center = new Vector3(Screen.width/2, Screen.height/2, positionerDistance);

		#if UNITY_EDITOR
		Ray ray = Camera.main.ScreenPointToRay (center);
		RaycastHit hit;

		//we'll try to hit one of the plane collider gameobjects that were generated by the plugin
		//effectively similar to calling HitTest with ARHitTestResultType.ARHitTestResultTypeExistingPlaneUsingExtent
		if (Physics.Raycast (ray, out hit, maxRayDistance, collisionLayerMask)) {
			//we're going to get the position from the contact point
			foundPlaneObject.transform.position = hit.point;

			//float distance = Vector3.Distance(foundPlaneObject.transform.position, Camera.main.transform.position);
			//Debug.Log("Distance " + distance);

			//and the rotation from the transform of the plane collider
			CurrentState = PositionerState.FoundPlane;
			foundPlaneObject.transform.rotation = hit.transform.rotation;

			// Turn on the found positioner graphics
			foundPlaneObject.SetActive(true);
			findingPlaneObject.SetActive(false);
			return;
		}


		#else
		var screenPosition = Camera.main.ScreenToViewportPoint(center);
		ARPoint point = new ARPoint {
		x = screenPosition.x,
		y = screenPosition.y
		};

		// prioritize reults types
		ARHitTestResultType[] resultTypes = {
		ARHitTestResultType.ARHitTestResultTypeExistingPlaneUsingExtent, 
		// if you want to use infinite planes use this:
		//ARHitTestResultType.ARHitTestResultTypeExistingPlane,
		//ARHitTestResultType.ARHitTestResultTypeHorizontalPlane, 
		//ARHitTestResultType.ARHitTestResultTypeFeaturePoint
		}; 

		foreach (ARHitTestResultType resultType in resultTypes)
		{
		if (HitTestWithResultType (point, resultType))
		{
		CurrentState = PositionerState.FoundPlane;
		return;
		}
		}
		#endif

		//if you got here, we have not found a plane, so if camera is facing below horizon, display the focus "finding" square
		if (trackingInitialized) {
			CurrentState = PositionerState.FindingPlane;
			foundPlaneObject.SetActive (false);
			findingPlaneObject.SetActive (true);

			//check camera forward is facing downward
			if (Vector3.Dot(Camera.main.transform.forward, Vector3.down) > -20)
			{

				//position the focus finding square a distance from camera and facing up
				findingPlaneObject.transform.position = Camera.main.ScreenToWorldPoint(center);

				//vector from camera to focussquare
				Vector3 vecToCamera = findingPlaneObject.transform.position - Camera.main.transform.position;

				//find vector that is orthogonal to camera vector and up vector
				Vector3 vecOrthogonal = Vector3.Cross(vecToCamera, Vector3.up);

				//find vector orthogonal to both above and up vector to find the forward vector in basis function
				Vector3 vecForward = Vector3.Cross(vecOrthogonal, Vector3.up);

				findingPlaneObject.transform.rotation = Quaternion.LookRotation(vecForward,Vector3.up);

			}
			else
			{
				//we will not display finding square if camera is not facing below horizon
				findingPlaneObject.SetActive(false);
			}
		}
	}

	/// <summary>
	/// Toggles the locking state of a positioner.  That way it can follow the
	/// camera, or stay at a fixed location.
	/// </summary>
	private void ToggleState() {
		switch (CurrentState) {
		case PositionerState.FindingPlane:
			Debug.Log ("Plane not detected, unable to place a graph here");
			break;
		case PositionerState.FoundPlane:
			PlaceGraph ();
			break;
		case PositionerState.Initializing:
			Debug.Log ("Unable to place a graph while initializing");
			break;
		case PositionerState.Locked:
			Debug.Log ("Graph already placed");
			break;
		}
		//			_currentAxisState = AxisState.Locked;
		//			var arInterface = UnityARSessionNativeInterface.GetARSessionNativeInterface ();
		//			AnchorID = arInterface.AddUserAnchorFromGameObject (this.gameObject).identifierStr;
	}

	/// <summary>
	/// Sets up and activates the graph, and disables the positioner
	/// </summary>
	private void PlaceGraph()
	{
		// Create and setup the new graph
		_graph = Instantiate (GraphPrefab);
		var graphManager = _graph.GetComponent<GraphManager> ();
		var graphScript = _graph.GetComponentInChildren(typeof(ResizeBox)) as ResizeBox;

		graphManager.TargetPosition = transform.position;

		// Register the graph to receive data updates from the provider
		// Sometime we will need to change this to let the user choose, and not hard code
		var availableTraces = DataProvider.AvailableTraces;
		graphScript.XTraceDescriptor = availableTraces [0];
		graphScript.YTraceDescriptor = availableTraces [1];
		graphScript.ZTraceDescriptor = availableTraces [2];
		DataProvider.RegisterForData (availableTraces [0], graphScript);
		DataProvider.RegisterForData (availableTraces [1], graphScript);
		DataProvider.RegisterForData (availableTraces [2], graphScript);

		// Turn off the positioning axes
		gameObject.SetActive(false);
		// Turn on the graph
		_graph.SetActive(true);
		return;
	}
}


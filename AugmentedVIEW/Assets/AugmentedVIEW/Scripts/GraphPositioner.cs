using UnityEngine;
using System.Collections;
using UnityEngine.XR.iOS;
using System.Collections.Generic;
using UnityEngine.UI;

public class GraphPositioner : MonoBehaviour
{
	public Text status; // Used for debugging on a device

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

	// Information for hit tests that can be used by the Unity editor or on a device
	private Vector3 hitPosition; // Used for recording a hit location
	private Quaternion hitRotation; // Used for recording a hit rotation

	//use center of screen for focusing
	private Vector3 center;

	// Game object for the positioner
	public GameObject findingPlaneObject;
	public GameObject foundPlaneObject;

	// Touch and placement info for the positioner
	public float maxRayDistance = 200.0f;
	public float positionerDistance = 0.5f;
	public LayerMask collisionLayerMask;

	public GameObject GraphPrefab { get; set; }
	private GameObject _graph { get; set; }

	private bool trackingInitialized;

	private bool HitTestWithResultType (ARPoint point, ARHitTestResultType resultTypes)
	{
		List<ARHitTestResult> hitResults = UnityARSessionNativeInterface.GetARSessionNativeInterface ().HitTest (point, resultTypes);
		if (hitResults.Count > 0) {
			foreach (var hitResult in hitResults) {
				hitPosition = UnityARMatrixOps.GetPosition (hitResult.worldTransform);
				hitRotation = UnityARMatrixOps.GetRotation (hitResult.worldTransform);
				Debug.Log (string.Format ("x:{0:0.######} y:{1:0.######} z:{2:0.######}", hitPosition.x, hitPosition.y, hitPosition.z));
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
		// Update this before updating the positioner location or checking for a user press
		center = new Vector3(Screen.width/2, Screen.height/2, positionerDistance);
		UpdatePositionerLocation ();
		CheckForUserPress ();
	}

	/// <summary>
	/// Checks to see if the user clicked on the graph positioner.  If they did,
	/// then the graph is positioned there, and we get rid of the positioner.
	/// </summary>
	private void CheckForUserPress()
	{
		bool userPressedScreen = false;
		Vector3 position = new Vector3(0,0,0);
		#if UNITY_EDITOR
		if (Input.GetMouseButtonDown (0)) {
			position = Input.mousePosition;
			userPressedScreen = true;
		} 
		#else
		foreach (var touch in Input.touches){
			if (touch.phase == TouchPhase.Began) {
				position = touch.position; // Using the last touch position... hope this is ok.
				userPressedScreen = true;
			}
		}
		#endif

		if (userPressedScreen)
		{
			RaycastHit hit;
			Ray userClickRay = Camera.main.ScreenPointToRay(position);
			if (Physics.Raycast(userClickRay, out hit)) 
			{
				ToggleState();
			}
		}
	}

	/// <summary>
	/// Updates the positioner location based on where the center of the screen is pointing.  If
	/// it is not pointing at a horizontal plane, then the state is set to finding plane.
	/// </summary>
	private void UpdatePositionerLocation()
	{
		status.color = Color.red;
		status.fontSize= 40;
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
				PositionerOnPlane ();
				status.text = "Found plane";
				return;
			}
		}
		status.text = "Nothing found in center of screen";

		//if you got here, we have not found a plane, so if camera is facing below horizon, display the focus "finding" square
		PositionerNotOnPlane();
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
			foundPlaneObject.SetActive (false); // turn off the positioner
			PlaceGraph ();
			break;
		case PositionerState.Initializing:
			Debug.Log ("Unable to place a graph while initializing");
			break;
		case PositionerState.Locked:
			Debug.Log ("Graph already placed");
			break;
		}
	}

	/// <summary>
	/// Sets up and activates the graph, and disables the positioner
	/// </summary>
	private void PlaceGraph()
	{
		// Create and setup the new graph
		_graph = Instantiate (GraphPrefab);
		var graphManager = _graph.GetComponent<GraphManager> ();

		graphManager.TargetPosition = transform.position;
		graphManager.TargetRotation = transform.rotation;

		// Turn off the positioning axes
		gameObject.SetActive(false);
		// Turn on the graph
		_graph.SetActive(true);
		return;
	}

	private void PositionerOnPlane()
	{
		// Set the state to found
		CurrentState = PositionerState.FoundPlane;

		// Set the found positioner to the location and rotation of where we found
		foundPlaneObject.transform.position = hitPosition;
		foundPlaneObject.transform.rotation = hitRotation;

		// Update object visibility to show the found plane and hide the finding plane
		findingPlaneObject.SetActive(false);
		foundPlaneObject.SetActive (true);

		status.text="Found valid plane for placing graph";
	}

	private void PositionerNotOnPlane()
	{
		if (trackingInitialized) {
			CurrentState = PositionerState.FindingPlane;
			foundPlaneObject.SetActive (false);
			findingPlaneObject.SetActive (true);

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
	}
}


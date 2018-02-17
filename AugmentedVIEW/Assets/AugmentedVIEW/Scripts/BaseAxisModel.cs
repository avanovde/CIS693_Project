using UnityEngine;
using System.Collections;
using UnityEngine.XR.iOS;

public class BaseAxisModel : MonoBehaviour
{
	private enum AxisState
	{
		Locked = 0,
		Selected
	}
	public float DistanceFromCamera;
	public float DistanceBelowCamera;

	public void SetGraphType(GameObject obj){
		_graphType = obj;
	}

	private AxisState _currentAxisState = AxisState.Selected;
	private Camera mainCamera;
	private IDataProvider _dataProvider;
	private GameObject _currentGraph;
	private GameObject _graphType;

	public string AnchorID { get; set; }

	void Start()
	{
		mainCamera = Camera.main;
	}

	void Update()
	{
		switch (_currentAxisState) 
		{
		case AxisState.Locked:
			break;
		case AxisState.Selected:
			// Update the location of the axis object
			var cameraTransform = mainCamera.transform;
			var cameraPosition = cameraTransform.position;

			transform.position = (cameraPosition + cameraTransform.forward * DistanceFromCamera);
			transform.position = (transform.position + transform.up * DistanceBelowCamera * -1);

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
		if (_currentAxisState == AxisState.Selected) {
			_currentAxisState = AxisState.Locked;
			var arInterface = UnityARSessionNativeInterface.GetARSessionNativeInterface ();
			AnchorID = arInterface.AddUserAnchorFromGameObject (this.gameObject).identifierStr;

			// Create and setup the new graph
			_currentGraph = Instantiate (_graphType);
			var graphManager = _currentGraph.GetComponent<GraphManager> ();
			var graphScript = _currentGraph.GetComponent<ResizeBox> ();

			graphManager.TargetPosition = transform.position;

			// Register the graph to receive data updates from the provider
			// Sometime we will need to change this to let the user choose, and not hard code
			var availableTraces = _dataProvider.AvailableTraces;
			graphScript.XTraceDescriptor = availableTraces [0];
			graphScript.YTraceDescriptor = availableTraces [1];
			graphScript.YTraceDescriptor = availableTraces [2];
			_dataProvider.RegisterForData (availableTraces [0], graphScript);
			_dataProvider.RegisterForData (availableTraces [1], graphScript);
			_dataProvider.RegisterForData (availableTraces [2], graphScript);
			
			// Turn off the positioning axes
			gameObject.SetActive(false);
			// Turn on the graph
			_currentGraph.SetActive(true);
			return;
		}
		if (_currentAxisState == AxisState.Locked) {
			_currentAxisState = AxisState.Selected;
			if (!AnchorID.Equals ("")) {
				UnityARSessionNativeInterface.GetARSessionNativeInterface ().RemoveUserAnchor (AnchorID);
				AnchorID = "";
			}
			return;
		}
	}

	/// <summary>
	/// Updates the current graph with the new provider, and stores the provider off
	/// for future use.
	/// </summary>
	/// <param name="updatedProvider">New provider to use.</param>
	public void UpdateDataProvider(IDataProvider updatedProvider)
	{
		//TODO Update an existing graph object with the new provider

		_dataProvider = updatedProvider;
	}
}


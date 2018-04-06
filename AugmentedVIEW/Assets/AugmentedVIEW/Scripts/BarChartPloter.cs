using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BarChartPloter : MonoBehaviour
{
	public int Channel; // Channel to pull data from
	public bool IsXAxis = false;
	public bool IsYAxis = false;
	public bool IsZAxis = false;

	public float GraphScaleFactor = 0.5f;

	private DataProvider _dataProvider;
	private ITraceDescriptor _traceDescriptor;


	private float _previousDataValue = 0.0f;
	private float _delta; // Change in the previous point

	private Vector3 _initialPosition;

	void Awake() {
		// First try get the data provider, before doing any more work
		GameObject providerObject = GameObject.Find("DataProvider");
		if (providerObject == null) {
			Debug.Log ("Unable to find the game object named data provider");
			return;
		}

		_dataProvider = providerObject.GetComponent<DataProvider> ();

		if (_dataProvider == null) {
			Debug.Log ("Unable to get the data provider");
			return;
		}

		// Set up which traces we want data from
		var tracesAvailable = _dataProvider.AvailableTraces;
		if (0 < Channel && Channel <= tracesAvailable.Count) {
			_traceDescriptor = tracesAvailable [Channel];		
		} else {
			Debug.Log ("Unable to locate trace descriptor for barchart");
		}
		_dataProvider.NewDataAvailable += HandleNewData;
	}

	// Use this for initialization
	void Start ()
	{
		_initialPosition = new Vector3 (0, 0, 0);
	}
	
	// Update is called once per frame
	void Update ()
	{
		if (_delta == 0.0f)
			return; // No change since last frame render, so drop out.
		
		var halfDelta = _delta / 2; // half because the center point is in the middle of the cube
		if (IsXAxis) {
			transform.position = new Vector3 (
				_initialPosition.x + halfDelta,
				_initialPosition.y,
				_initialPosition.z
			);

			transform.localScale += new Vector3(_delta, 0, 0);
		} else if (IsYAxis) {
			transform.position = new Vector3 (
				_initialPosition.x,
				_initialPosition.y + halfDelta,
				_initialPosition.z
			);

			transform.localScale += new Vector3(0, _delta, 0);
		} else if (IsZAxis) {
			transform.position = new Vector3 (
				_initialPosition.x,
				_initialPosition.y,
				_initialPosition.z + halfDelta
			);

			transform.localScale += new Vector3(0, 0, _delta);
		}

		// reset delta to show we used its value
		_delta = 0.0f;
	}

	void OnDestroy() {
		_dataProvider.NewDataAvailable -= HandleNewData;
	}

	public void HandleNewData (
		object sender,
		DataUpdatedEventArgs e)
	{
		var data = e.Data;

		for (int channelIndex = 0; channelIndex < data.values.Count; channelIndex++) {
			if (channelIndex == _traceDescriptor.Channel) {
				//Debug.Log ("X: " + newData);
				var newDataPoint = data.values [channelIndex] * GraphScaleFactor;
				_delta = _previousDataValue - newDataPoint;
				_previousDataValue = newDataPoint;
			}
		}
	}
}


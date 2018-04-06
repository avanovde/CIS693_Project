using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class MoveBox : MonoBehaviour
{
	public float GraphScaleFactor = 0.5f;
	public float Speed = 1.0f;

	public ITraceDescriptor XTraceDescriptor;
	public ITraceDescriptor YTraceDescriptor;
	public ITraceDescriptor ZTraceDescriptor;

	// Current set of X, Y, and Z values for the graph
	private float _xValue;
	private float _yValue;
	private float _zValue;

	public bool GraphPositioned = false;
	private float _graphMoveFactor;

	private Vector3 _currentPosition;
	private Vector3 _targetPosition;

	private DataProvider _dataProvider;


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
		XTraceDescriptor = tracesAvailable [0];
		YTraceDescriptor = tracesAvailable [1];
		ZTraceDescriptor = tracesAvailable [2];

		_dataProvider.NewDataAvailable += HandleNewData;
	}

	// Use this for initialization
	void Start () {
		transform.position = new Vector3 (0, 0, 0); // MoveBox us Touch the origin.
		_currentPosition = transform.position;
		_targetPosition = transform.position;
	}

	// Update is called once per frame
	void Update () {
		_graphMoveFactor = Time.deltaTime * Speed;
		transform.position = Vector3.Lerp (_currentPosition, _targetPosition, _graphMoveFactor);
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
			if ( channelIndex == XTraceDescriptor.Channel) {
				//Debug.Log ("X: " + data.values[channelIndex]);
				_xValue = data.values[channelIndex] * GraphScaleFactor;
			} else if (channelIndex == YTraceDescriptor.Channel) {
				//Debug.Log ("Y: " + data.values[channelIndex]);
				_yValue = data.values[channelIndex] * GraphScaleFactor;
			} else if (channelIndex == ZTraceDescriptor.Channel) {
				//Debug.Log ("Z: " + data.values[channelIndex]);
				_zValue = data.values[channelIndex] * GraphScaleFactor;
			}
		}

		_targetPosition = new Vector3 (_xValue, _yValue, _zValue);
	}
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ResizeBox : MonoBehaviour
{
	public float GraphScaleFactor = 0.5f;

	public ITraceDescriptor XTraceDescriptor;
	public ITraceDescriptor YTraceDescriptor;
	public ITraceDescriptor ZTraceDescriptor;

	// Current set of X, Y, and Z values for the graph
	private float _xValue;
	private float _yValue;
	private float _zValue;

	public bool GraphPositioned = false;
	private float _graphMoveFactor;

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
	}
	
	// Update is called once per frame
	void Update () {
		transform.localScale = new Vector3(1 + _xValue, 1 + _yValue, 1 + _zValue);
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
			if ( channelIndex == XTraceDescriptor.ChannelIndex) {
				//Debug.Log ("X: " + newData);
				// Absolute value so we don't implode when acceleration is negative
				// Add .001 so that it doesn't ever reach 0
				_xValue = Math.Abs(data.values[channelIndex] * GraphScaleFactor) + 0.001f;
			} else if (channelIndex == YTraceDescriptor.ChannelIndex) {
				//Debug.Log ("Y: " + newData);
				// Absolute value so we don't implode when acceleration is negative
				// Add .001 so that it doesn't ever reach 0
				_yValue = Math.Abs(data.values[channelIndex] * GraphScaleFactor) + 0.001f;
			} else if (channelIndex == ZTraceDescriptor.ChannelIndex) {
				//Debug.Log ("Z: " + newData);
				// Absolute value so we don't implode when acceleration is negative
				// Add .001 so that it doesn't ever reach 0
				_zValue = Math.Abs(data.values[channelIndex] * GraphScaleFactor) + 0.001f;
			}
		}
	}
}

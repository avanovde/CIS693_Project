using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BarChartPloter : MonoBehaviour
{
	public int Channel; // Channel to pull data from
	public bool IsXAxis = false;
	public bool IsYAxis = false;
	public bool IsZAxis = false;

	public float GraphScaleFactor = 0.25f;

	private DataProvider _dataProvider;
	private ITraceDescriptor _traceDescriptor;


	private float _dataValue = 0.0f;

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
		foreach (var currentTrace in tracesAvailable) {
			if (currentTrace.ChannelIndex == Channel)
				_traceDescriptor = currentTrace;
		}

		if (_traceDescriptor == null) {
			Debug.Log ("Unable to locate trace descriptor");
			return;
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
		var autoScaleX = transform.localScale.x;
		var autoScaleY = transform.localScale.y;
		var autoScaleZ = transform.localScale.z;

		if (IsXAxis) {
			transform.position = new Vector3 (
				_dataValue * 0.5f,
				_initialPosition.y,
				_initialPosition.z
			);

			transform.localScale = new Vector3(_dataValue, autoScaleY, autoScaleZ);
		} else if (IsYAxis) {
			transform.position = new Vector3 (
				_initialPosition.x,
				_dataValue * 0.5f,
				_initialPosition.z
			);

			transform.localScale = new Vector3(autoScaleX, _dataValue, autoScaleZ);
		} else if (IsZAxis) {
			transform.position = new Vector3 (
				_initialPosition.x,
				_initialPosition.y,
				_dataValue * 0.5f
			);

			transform.localScale = new Vector3(autoScaleX, autoScaleY, _dataValue);
		}
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
			if (channelIndex == _traceDescriptor.ChannelIndex) {
				//Debug.Log ("X: " + newData);
				
				_dataValue = data.values [channelIndex] * GraphScaleFactor;
			}
		}
	}
}


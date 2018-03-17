using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ResizeBox : MonoBehaviour, IDataProcessor
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


	// Use this for initialization
	void Start () {
		_currentPosition = transform.position;
		_targetPosition = transform.position;
	}
	
	// Update is called once per frame
	void Update () {
		_graphMoveFactor = Time.deltaTime * Speed;
		transform.position = Vector3.Lerp (_currentPosition, _targetPosition, _graphMoveFactor);
	}

	#region IDataProcessor implementation

	public void DataUpdated (
		ITraceDescriptor traceDescriptor,
		float newData)
	{
		if (traceDescriptor.Channel == XTraceDescriptor.Channel) {
			//Debug.Log ("X: " + newData);
			_xValue = newData * GraphScaleFactor;
		} else if (traceDescriptor.Channel == YTraceDescriptor.Channel) {
			//Debug.Log ("Y: " + newData);
			_yValue = newData * GraphScaleFactor;
		} else if (traceDescriptor.Channel == ZTraceDescriptor.Channel) {
			//Debug.Log ("Z: " + newData);
			_zValue = newData * GraphScaleFactor;
		} else {
			Debug.Log ("Unused trace descriptor applied to resize box");
			return;
		}

		_targetPosition = new Vector3 (_xValue, _yValue, _zValue);
	}

	#endregion
}

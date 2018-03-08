﻿using System.Collections;
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

	// Previous X, Y, and Z values so we can calculate how far to move the cube
	private float _previousXValue;
	private float _previousYValue;
	private float _previousZValue;

	// Current set of X, Y, and Z values for the graph
	private float _xValue;
	private float _yValue;
	private float _zValue;

	public bool GraphPositioned = false;

	private Vector3 _currentPosition;
	private Vector3 _targetPosition;


	// Use this for initialization
	void Start () {
		_currentPosition = transform.position;
		_targetPosition = transform.position;
	}
	
	// Update is called once per frame
	void Update () {
		float graphMoveFactor = Time.deltaTime * Speed;
		//Debug.Log ("Current: " + _currentPosition + " Target: " + _targetPosition);
		Vector3.Lerp (_currentPosition, _targetPosition, graphMoveFactor);
	}

	#region IDataProcessor implementation

	public void DataUpdated (
		ITraceDescriptor traceDescriptor,
		float newData)
	{
		if (traceDescriptor.Channel == XTraceDescriptor.Channel) {
			//Debug.Log ("X: " + newData);
			_previousXValue = _xValue;
			_xValue = newData * GraphScaleFactor;
		} else if (traceDescriptor.Channel == YTraceDescriptor.Channel) {
			//Debug.Log ("Y: " + newData);
			_previousYValue = _yValue;
			_yValue = newData * GraphScaleFactor;
		} else if (traceDescriptor.Channel == ZTraceDescriptor.Channel) {
			//Debug.Log ("Z: " + newData);
			_previousZValue = _zValue;
			_zValue = newData * GraphScaleFactor;
		} else {
			Debug.Log ("Unused trace descriptor applied to resize box");
			return;
		}

		float deltaX = _previousXValue - _xValue;
		float deltaY = _previousYValue - _yValue;
		float deltaZ = _previousZValue - _zValue;

		_currentPosition = transform.position;

		if (deltaX != 0f || deltaY != 0 || deltaZ != 0)
			_targetPosition = new Vector3 (deltaX, deltaY, deltaZ);
	}

	#endregion
}

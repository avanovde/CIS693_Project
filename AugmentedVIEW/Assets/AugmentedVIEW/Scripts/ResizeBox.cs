﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ResizeBox : MonoBehaviour, IDataProcessor
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

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
		transform.localScale = new Vector3(1 + _xValue, 1 + _yValue, 1 + _zValue);
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
	}

	#endregion
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResizeBox : MonoBehaviour, IDataProcessor
{

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

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {

		float deltaX = _previousXValue - _xValue;
		float deltaY = _previousYValue - _yValue;
		float deltaZ = _previousZValue - _zValue;

		if (deltaX != 0f || deltaY != 0 || deltaZ != 0)
			transform.Translate (new Vector3 (deltaX, deltaY, deltaZ));
	}

	#region IDataProcessor implementation

	public void DataUpdated (
		ITraceDescriptor traceDescriptor,
		float newData)
	{
		if (traceDescriptor.Equals(XTraceDescriptor)) {
			_previousXValue = _xValue;
			_xValue = newData;
		} else if (traceDescriptor.Equals(YTraceDescriptor)) {
			_previousYValue = _yValue;
			_yValue = newData;
		} else if (traceDescriptor.Equals(ZTraceDescriptor)) {
			_previousZValue = _zValue;
			_zValue = newData;
		} else {
			Debug.Log("Unused trace descriptor applied to Resize Box");
		}
	}

	#endregion
}

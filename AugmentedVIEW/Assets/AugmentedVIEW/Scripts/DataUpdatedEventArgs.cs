using UnityEngine;
using System.Collections;
using System;

public class DataUpdatedEventArgs : EventArgs
{
	// Consider making read only for data integrity's sake
	public float DataPoint { get; set;}
	public int TraceId { get; set; }
}


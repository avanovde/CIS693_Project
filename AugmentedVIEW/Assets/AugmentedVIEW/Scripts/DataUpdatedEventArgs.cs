using UnityEngine;
using System.Collections;
using System;

public class DataUpdatedEventArgs : EventArgs
{
	public DataUpdatedEventArgs(IOData data)
	{
		Data = data;
	}

	public IOData Data { get; }
}


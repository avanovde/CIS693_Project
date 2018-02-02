using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DataProvider : MonoBehaviour, IDataProvider
{
	// Mapping of data to graph
	Dictionary<ITraceDescriptor, IDataProcessor> _dataProcessorDictionary = 
		new Dictionary<ITraceDescriptor, IDataProcessor>();

	List<ITraceDescriptor> _tracesAvailable =
		new List<ITraceDescriptor>();
	
	// Use this for initialization
	void Start ()
	{
		// Stubbing out the trace descriptors that are available for consumption
		_tracesAvailable.Add (new TraceDescriptor (1, DataType.Time));
		_tracesAvailable.Add (new TraceDescriptor (1, DataType.FFT));
		_tracesAvailable.Add (new TraceDescriptor (2, DataType.Time));
		_tracesAvailable.Add (new TraceDescriptor (2, DataType.FFT));
		_tracesAvailable.Add (new TraceDescriptor (3, DataType.Time));
		_tracesAvailable.Add (new TraceDescriptor (3, DataType.FFT));
	}
	
	// Update is called once per frame
	void Update ()
	{
	
	}

	#region IDataProvider implementation
	public IList<ITraceDescriptor> AvailableTraces 
	{ 
		get { return new List<ITraceDescriptor> (_tracesAvailable); }
	}

	public void RegisterForData (
		ITraceDescriptor traceDescriptor, 
		IDataProcessor newProcessor)
	{
		if (!_dataProcessorDictionary.ContainsKey (traceDescriptor))
			_dataProcessorDictionary.Add (traceDescriptor, newProcessor);
		else
			Debug.Log ("Trace descriptor already registered");
	}

	public void DeregisterForData (
		ITraceDescriptor traceDescriptor)
	{
		if (_dataProcessorDictionary.ContainsKey (traceDescriptor))
			_dataProcessorDictionary.Remove (traceDescriptor);
	}

	#endregion
}


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

	private string jsonString = @"{
		""values"": [
			1.6552945375442505,
			1.8027756214141846,
			2.2693612575531008,
			2.6514148712158205,
			3.58050274848938,
			3.8626415729522707,
			4.253233909606934,
			4.591296195983887,
			4.710626125335693,
			5.858327388763428,
			5.674504280090332,
			5.544366359710693,
			5.574047088623047,
			5.257375717163086,
			5.21056604385376,
			4.928488731384277
		]
}";

	
	// Use this for initialization
	void Start ()
	{
		// Stubbing out the trace descriptors that are available for consumption
		_tracesAvailable.Add (new TraceDescriptor (1, DataType.Time));
		//_tracesAvailable.Add (new TraceDescriptor (1, DataType.FFT));
		_tracesAvailable.Add (new TraceDescriptor (2, DataType.Time));
		//_tracesAvailable.Add (new TraceDescriptor (2, DataType.FFT));
		_tracesAvailable.Add (new TraceDescriptor (3, DataType.Time));
		//_tracesAvailable.Add (new TraceDescriptor (3, DataType.FFT));
	}
	
	// Update is called once per frame
	// Hopefully doing a HTTP get once per frame, that may be optemistic
	void Update ()
	{
		var data = IOData.CreateFromJSON (jsonString);
		foreach (var traceDescriptor in _tracesAvailable) {
			if (data.values.Count < traceDescriptor.Channel) {
				float dataPoint = data.values [traceDescriptor.Channel - 1]; // Subtract 1 since channel and list have different origin indexes
				if (_dataProcessorDictionary.ContainsKey (traceDescriptor))
					_dataProcessorDictionary [traceDescriptor].DataUpdated (traceDescriptor, dataPoint);
			}
		}

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

//https://docs.unity3d.com/ScriptReference/JsonUtility.FromJson.html
[System.Serializable]
public class IOData
{
	public List<float> values;

	public static IOData CreateFromJSON(string jsonString)
	{
		return JsonUtility.FromJson<IOData> (jsonString);
	}
}


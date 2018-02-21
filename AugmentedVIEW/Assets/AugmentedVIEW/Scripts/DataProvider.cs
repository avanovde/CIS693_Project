using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DataProvider : MonoBehaviour, IDataProvider
{
	// Mapping of data to graph
	Dictionary<int, IDataProcessor> _dataProcessorDictionary = 
		new Dictionary<int, IDataProcessor>();

	Dictionary<int, ITraceDescriptor> _tracesAvailable =
		new Dictionary<int, ITraceDescriptor>();

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
		_tracesAvailable.Add (0, new TraceDescriptor (1, DataType.Time));
		//_tracesAvailable.Add (new TraceDescriptor (1, DataType.FFT));
		_tracesAvailable.Add (1, new TraceDescriptor (2, DataType.Time));
		//_tracesAvailable.Add (new TraceDescriptor (2, DataType.FFT));
		_tracesAvailable.Add (2, new TraceDescriptor (3, DataType.Time));
		//_tracesAvailable.Add (new TraceDescriptor (3, DataType.FFT));
	}
	
	// Update is called once per frame
	// Hopefully doing a HTTP get once per frame, that may be optemistic
	void Update ()
	{
		var data = IOData.CreateFromJSON (jsonString);
		foreach (var traceDescriptor in _tracesAvailable.Values) {
			if (0 <= traceDescriptor.Channel && traceDescriptor.Channel <= _tracesAvailable.Count) {
				float dataPoint = data.values [traceDescriptor.Channel];
				if (_dataProcessorDictionary.ContainsKey (traceDescriptor.Channel))
					_dataProcessorDictionary [traceDescriptor.Channel].DataUpdated (traceDescriptor, dataPoint);
			}
		}

	}

	#region IDataProvider implementation
	public IList<ITraceDescriptor> AvailableTraces 
	{ 
		get { return new List<ITraceDescriptor> (_tracesAvailable.Values); }
	}

	public void RegisterForData (
		ITraceDescriptor traceDescriptor, 
		IDataProcessor newProcessor)
	{
		if (!_dataProcessorDictionary.ContainsKey (traceDescriptor.Channel) && _tracesAvailable.ContainsKey(traceDescriptor.Channel) )
			_dataProcessorDictionary.Add (traceDescriptor.Channel, newProcessor);
		else
			Debug.Log ("Trace descriptor already registered");
	}

	public void DeregisterForData (
		ITraceDescriptor traceDescriptor)
	{
		if (_dataProcessorDictionary.ContainsKey (traceDescriptor.Channel))
			_dataProcessorDictionary.Remove (traceDescriptor.Channel);
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


using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;
using System.Text.RegularExpressions;
using System;

public class DataProvider : MonoBehaviour
{
	Dictionary<int, ITraceDescriptor> _tracesAvailable =
		new Dictionary<int, ITraceDescriptor>();

	UnityWebRequest webRequest; 

	// Box communication variables
	private DownloadHandler _downloadHandler;
	private IEnumerator reqco;

	private string _ipAddress = "192.168.30.1";
	private float _dataScalar = 0.1f;

	public event EventHandler<DataUpdatedEventArgs> NewDataAvailable;

	// Use this for initialization
	void Start ()
	{
		// Stubbing out the trace descriptors that are available for consumption
		_tracesAvailable.Add (1, new TraceDescriptor (1, DataType.Time)); // X
		_tracesAvailable.Add (2, new TraceDescriptor (2, DataType.Time)); // Y
		_tracesAvailable.Add (3, new TraceDescriptor (3, DataType.Time)); // z

		reqco = GetNewData ();
		StartCoroutine (reqco);
	}

	private IEnumerator GetNewData()
	{
		while (true) {
			using (UnityWebRequest request = UnityWebRequest.Get ("http://" + _ipAddress + ":80/api/channeldata/currentvalues")) {
				request.chunkedTransfer = false;

				yield return request.SendWebRequest ();
				if (request.isHttpError || request.isNetworkError) {
					Debug.Log (request.error);
				} else {
					if (request.isDone) {
						string data = System.Text.Encoding.UTF8.GetString (request.downloadHandler.data);
						//Debug.Log ("Data: " + data);
						var parsedData = IOData.CreateFromJSON (data);
						foreach (var traceDescriptor in _tracesAvailable.Values) {
							if (0 <= traceDescriptor.Channel && traceDescriptor.Channel <= parsedData.values.Count) {
								float dataPoint = parsedData.values [traceDescriptor.Channel] * _dataScalar;
								var dataUpdatedEventArgs = new DataUpdatedEventArgs ();
								dataUpdatedEventArgs.DataPoint = dataPoint;
								dataUpdatedEventArgs.TraceId = traceDescriptor.Channel;
								OnNewDataAvailable (dataUpdatedEventArgs);
							}
						}
					}
				}
			}
		}
	}

	protected virtual void OnNewDataAvailable(DataUpdatedEventArgs e)
	{
		EventHandler<DataUpdatedEventArgs> handler = NewDataAvailable;
		if (handler != null) {
			handler (this, e);
		}
	}

	public void OnServerAddressChanged(string newAddress)
	{
		string pattern = "\\b\\d{1,3}\\.\\d{1,3}\\.\\d{1,3}\\.\\d{1,3}\\b";
		Regex regex = new Regex (pattern);
		Match regMatch = regex.Match (newAddress);

		if (regMatch.Success)
			_ipAddress = newAddress;
	} 

	public void OnUpdateDataScalar(float value)
	{
		_dataScalar = value * 3.0f;
	}

	void OnDestroy()
	{
		StopCoroutine (reqco);
	}
		
	public IList<ITraceDescriptor> AvailableTraces 
	{ 
		get { return new List<ITraceDescriptor> (_tracesAvailable.Values); }
	}
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


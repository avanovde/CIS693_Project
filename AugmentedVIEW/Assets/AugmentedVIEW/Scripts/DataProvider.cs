using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;
using System.Text.RegularExpressions;
using System;
using System.Text;

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
			//using (UnityWebRequest request = UnityWebRequest.Get ("http://" + _ipAddress + ":80/api/channeldata/currentvalues")) {
			using (UnityWebRequest request = UnityWebRequest.Get ("http://" + _ipAddress + ":80/api/channeldata/latest-data?n-pts=1000")) {
				request.chunkedTransfer = false;
				float[] dataPoints = new float[1000]; // Only get the last 100 samples. This may change.

				yield return request.SendWebRequest ();
				if (request.isHttpError || request.isNetworkError) {
					Debug.Log (request.error);
				} else {
					if (request.isDone) {
						string data = System.Text.Encoding.UTF8.GetString (request.downloadHandler.data);
						//Debug.Log ("Data: " + data);
						var parsedData = RootObject.CreateFromJSON(data);
						var channelCount = parsedData.channels.Count;

						var ioData = new IOData();
						ioData.values = new List<float>(channelCount);

						for (var channelIndex = 0; channelIndex < channelCount; channelIndex++) {
							ioData.values.Add (0.0f);
							var hiString = parsedData.channels [channelIndex].hi;
							// Only proceed if we have data for this channel
							if (hiString != string.Empty) {
								// Convert from a string to a float array
								byte[] byteArray = Convert.FromBase64String(hiString);//ASCIIEncoding.ASCII.GetBytes (hiString.Substring(0, dataPoints.Length * sizeof(float)));
								Buffer.BlockCopy (byteArray, 0, dataPoints, 0, dataPoints.Length * sizeof(float));
								float maxValue = float.MinValue;
								// Find the maximum value in the data
								foreach (var currentValue in dataPoints) {
									if (currentValue > maxValue)
										maxValue = currentValue;
								}
								ioData.values[channelIndex] = maxValue;

							}
						}

						var dataUpdatedEventArgs = new DataUpdatedEventArgs (ioData);
						OnNewDataAvailable (dataUpdatedEventArgs);
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
public class IOData
{
	public List<float> values { get; set; }
}

[System.Serializable]
public class Channel
{
	public string lo;
	public string hi;
}

[System.Serializable]
public class RootObject
{
	public int sampleRate;
	public double samplesPerPt;
	public double firstTime;
	public List<Channel> channels;

	public static RootObject CreateFromJSON(string jsonString)
	{
		return JsonUtility.FromJson<RootObject> (jsonString);
	}
}


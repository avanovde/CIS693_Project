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

	public int UpdateScaleFactor = 5;

	private static string s1 = @"{
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
	private static string s2 = @"{
		""values"": [
	        5.134199142456055,
	        5.222068786621094,
	        4.769696235656738,
	        4.509988784790039,
	        4.039802074432373,
	        3.5185225009918215,
	        3.016620635986328,
	        2.2158520221710207,
	        1.723368763923645,
	        1.3892444372177125,
	        1.4387494325637818,
	        1.726267695426941,
	        2.2561028003692629,
	        2.731300115585327,
	        3.44673752784729,
	        4.291852951049805
		]
}";
	private static string s3 = @"{
		""values"": [
			2.929163694381714,
	        2.1095023155212404,
	        1.746424913406372,
	        1.5132745504379273,
	        1.5,
	        1.8275666236877442,
	        2.3430747985839845,
	        2.896549701690674,
	        3.5014283657073976,
	        3.940812110900879,
	        4.471017837524414,
	        4.699999809265137,
	        5.298112869262695,
	        5.1797685623168949,
	        5.707889556884766,
	        5.518151760101318
		]
}";
	private static string s4 = @"{
		""values"": [
			5.509083271026611,
	        4.752893924713135,
	        4.651881217956543,
	        4.213074684143066,
	        4.017461776733398,
	        3.1080539226531984,
	        2.831960439682007,
	        2.280350923538208,
	        1.7999999523162842,
	        1.6703293323516846,
	        1.9313207864761353,
	        2.0976176261901857,
	        2.7694764137268068,
	        3.1543619632720949,
	        3.691883087158203,
	        4.199999809265137
		]
}";
	private static string s5 = @"{
		""values"": [
			5.3842363357543949,
	        5.727128505706787,
	        5.570457935333252,
	        5.504543304443359,
	        5.588380813598633,
	        4.74025297164917,
	        4.642197608947754,
	        4.494441032409668,
	        3.670149803161621,
	        3.410278558731079,
	        3.243454933166504,
	        2.253885507583618,
	        1.5811388492584229,
	        1.403566837310791,
	        1.4142135381698609,
	        1.9078783988952637
		]
}";
	private static string s6 = @"{
		""values"": [
			3.5,
	        3.3882148265838625,
	        2.8618175983428957,
	        2.5612497329711916,
	        2.3643181324005129,
	        2.3600847721099855,
	        2.5729360580444338,
	        3.6687872409820558,
	        4.535416126251221,
	        4.7874836921691898,
	        4.828042984008789,
	        5.226853847503662,
	        5.54707145690918,
	        5.670097351074219,
	        5.348831653594971,
	        5.8991522789001469
		]
}";
	private static string s7 = @"{
		""values"": [
			5.968249320983887,
	        5.391660213470459,
	        5.822371006011963,
	        5.0596442222595219,
	        4.253233909606934,
	        4.565084934234619,
	        3.920459032058716,
	        3.7067506313323976,
	        3.1874754428863527,
	        2.758622884750366,
	        2.594224452972412,
	        2.5278449058532717,
	        2.291287899017334,
	        2.289104461669922,
	        2.580697536468506,
	        2.817800521850586
		]
}";
	private static string s8 = @"{
		""values"": [
			4.956813335418701,
	        4.620605945587158,
	        4.31740665435791,
	        3.370459794998169,
	        3.25883412361145,
	        2.668332815170288,
	        2.102379560470581,
	        2.8792359828948976,
	        3.651027202606201,
	        3.5860841274261476,
	        4.697871685028076,
	        4.760251998901367,
	        4.9789557456970219,
	        4.980963706970215,
	        5.162363529205322,
	        5.035871505737305
		]
}";
	private string[] jsonString;
	private int index;
	
	// Use this for initialization
	void Start ()
	{
		index = 0;
		jsonString = new string[] { s1, s2, s3, s4, s5, s6, s7, s8 };
		// Stubbing out the trace descriptors that are available for consumption
		_tracesAvailable.Add (1, new TraceDescriptor (1, DataType.Time));
		//_tracesAvailable.Add (new TraceDescriptor (1, DataType.FFT));
		_tracesAvailable.Add (2, new TraceDescriptor (2, DataType.Time));
		//_tracesAvailable.Add (new TraceDescriptor (2, DataType.FFT));
		_tracesAvailable.Add (3, new TraceDescriptor (3, DataType.Time));
		//_tracesAvailable.Add (new TraceDescriptor (3, DataType.FFT));
	}
	
	// Update is called once per frame
	// Hopefully doing a HTTP get once per frame, that may be optemistic
	void Update ()
	{
		var data = IOData.CreateFromJSON (jsonString[(index / UpdateScaleFactor) % jsonString.Length]);
		foreach (var traceDescriptor in _tracesAvailable.Values) {
			if (0 <= traceDescriptor.Channel && traceDescriptor.Channel <= data.values.Count) {
				float dataPoint = data.values [traceDescriptor.Channel];
				if (_dataProcessorDictionary.ContainsKey (traceDescriptor.Channel))
					_dataProcessorDictionary [traceDescriptor.Channel].DataUpdated (traceDescriptor, dataPoint);
			}
		}

		index++;
		if (index / UpdateScaleFactor > data.values.Count) {
			
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


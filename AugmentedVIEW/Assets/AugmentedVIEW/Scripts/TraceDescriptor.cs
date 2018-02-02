public class TraceDescriptor : ITraceDescriptor
{
	private int _channel;
	private DataType _dataType;
	public TraceDescriptor (
		int channel,
		DataType dataType)
	{
		_channel = channel;
		_dataType = dataType;
	}
	#region ITraceDescriptor implementation
	int Channel 
	{
		get { return _channel; }
	}

	DataType DataType 
	{ 
		get { return _dataType; }
	}
	#endregion
}


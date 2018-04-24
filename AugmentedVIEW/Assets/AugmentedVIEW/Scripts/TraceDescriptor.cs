public class TraceDescriptor : ITraceDescriptor
{
	public TraceDescriptor (
		int channel,
		DataType dataType)
	{
		ChannelIndex = channel;
		TraceDataType = dataType;
	}

	#region ITraceDescriptor implementation

	// Index of the channel on the IOBox
	public int ChannelIndex { get; }
	// Type of data belonging to the particular trace
	public DataType TraceDataType { get; }

	#endregion
}


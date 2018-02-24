public class TraceDescriptor : ITraceDescriptor
{
	public TraceDescriptor (
		int channel,
		DataType dataType)
	{
		Channel = channel;
		TraceDataType = dataType;
	}

	#region ITraceDescriptor implementation

	// Index of the channel on the IOBox
	public int Channel { get; }
	// Type of data belonging to the particular trace
	public DataType TraceDataType { get; }

	#endregion
}


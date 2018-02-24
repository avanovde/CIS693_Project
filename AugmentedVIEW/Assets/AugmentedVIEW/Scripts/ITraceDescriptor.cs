public interface ITraceDescriptor
{
	// Channel index of the trace on the IO Box
	int Channel { get; }
	// Type of data the descriptor is associated with
	DataType TraceDataType { get; }
}


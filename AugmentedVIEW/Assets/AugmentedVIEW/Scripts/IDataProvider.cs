using System.Collections.Generic;

public interface IDataProvider
{
	// List of traces available to be consumed by a graph
	IList<ITraceDescriptor> AvailableTraces { get; }

	// Register a graph to receive updated data when new data is available from the trace
	void RegisterForData (
		ITraceDescriptor traceDescriptor,
		IDataProcessor newProcessor
		);

	// Remove a graph from getting updated data for a trace
	void DeregisterForData (ITraceDescriptor traceDescriptor);
}
	
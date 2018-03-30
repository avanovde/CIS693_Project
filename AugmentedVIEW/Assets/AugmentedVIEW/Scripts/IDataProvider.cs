using System.Collections.Generic;
using System;

public interface IDataProvider
{
	// List of traces available to be consumed by a graph
	IList<ITraceDescriptor> AvailableTraces { get; }

	event EventHandler<DataUpdatedEventArgs> NewDataAvailable;
}
	
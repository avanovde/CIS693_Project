using System;

public interface IDataProcessor
{
	void DataUpdated(ITraceDescriptor channel, float newData);
}


using System.Globalization;

namespace Pinewolytics.Hubs;

public class RealtimeValue : Attribute
{
	public string Key { get; }
	public int MillisecondInterval { get; }

	public RealtimeValue(string key, int millisecondInterval)
	{
		Key = key;
		MillisecondInterval = millisecondInterval;
	}
}

namespace Pinewolytics.Hubs;

public class RealtimeValue : Attribute
{
	public int MillisecondInterval { get; }
	public string Name { get; }

	public RealtimeValue(int millisecondInterval, string name)
	{
		Name = name;
		MillisecondInterval = millisecondInterval;
	}
}

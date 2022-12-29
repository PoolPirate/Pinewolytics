namespace Pinewolytics.Models;

public class FlipsidePrimitiveObject<T> : IFlipsideObject<FlipsidePrimitiveObject<T>>
    where T :
    IComparable, IComparable<T?>, IConvertible, IEquatable<T?>
{
    public required T Value { get; init; }

    public static FlipsidePrimitiveObject<T> Parse(string[] rawValues)
    {
        if (!typeof(T).IsPrimitive && typeof(T) != typeof(string))
        {
            throw new ArgumentException("FlipsidePrimitiveObject only accepts primitive types!");
        }
        if (rawValues.Length != 1)
        {
            throw new ArgumentException($"Expecting exactly one column, got {rawValues.Length}");
        }
        //
        return new FlipsidePrimitiveObject<T>()
        {
            Value = (T)Convert.ChangeType(rawValues[0], typeof(T))
        };
    }
}

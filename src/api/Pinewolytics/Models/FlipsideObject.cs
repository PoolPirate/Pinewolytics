namespace Pinewolytics.Models;

public interface IFlipsideObject<T>
{
    public static abstract T Parse(string[] rawValues);
}

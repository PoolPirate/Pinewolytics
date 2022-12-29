namespace Pinewolytics.Models;

public interface IFlipsideObject<T>
{
    public abstract static T Parse(string[] rawValues);
}

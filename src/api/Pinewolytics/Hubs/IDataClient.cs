namespace Pinewolytics.Hubs;

public interface IDataClient
{
    public string[] GetPropertyNames();
    public object GetPropertyValue(string propertyName);
}

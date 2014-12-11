namespace MassTransit.Internals.Mapping
{
    public interface IObjectValueProvider
    {
        bool TryGetValue(string name, out object value);
        bool TryGetValue<T>(string name, out T value);
    }
}
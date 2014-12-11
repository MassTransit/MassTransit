namespace MassTransit.Internals.Mapping
{
    public interface IArrayValueProvider
    {
        bool TryGetValue(int index, out object value);
        bool TryGetValue<T>(int index, out T value);
    }
}
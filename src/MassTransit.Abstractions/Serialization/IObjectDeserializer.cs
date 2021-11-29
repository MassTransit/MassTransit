#nullable enable
namespace MassTransit
{
    public interface IObjectDeserializer
    {
        T? DeserializeObject<T>(object? value, T? defaultValue = null)
            where T : class;

        T? DeserializeObject<T>(object? value, T? defaultValue = null)
            where T : struct;
    }
}

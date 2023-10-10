#nullable enable
namespace MassTransit.Internals
{
    using System.Diagnostics.CodeAnalysis;
    using System.Reflection;


    public interface IReadPropertyCache<T>
        where T : class
    {
        IReadProperty<T, TProperty> GetProperty<TProperty>(string? name);
        IReadProperty<T, TProperty> GetProperty<TProperty>(PropertyInfo? propertyInfo);
        bool TryGetProperty<TProperty>(string name, [NotNullWhen(true)] out IReadProperty<T, TProperty>? property);
    }
}

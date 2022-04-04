namespace MassTransit.Internals
{
    using System.Collections.Generic;


    public interface IReadOnlyPropertyCache<T> :
        IEnumerable<ReadOnlyProperty<T>>
    {
        bool TryGetValue(string key, [NotNullWhen(true)] out ReadOnlyProperty<T>? value);
    }
}

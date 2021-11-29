namespace MassTransit.Internals
{
    public interface ITypeCache<T>
    {
        string ShortName { get; }
        IReadOnlyPropertyCache<T> ReadOnlyPropertyCache { get; }
        IReadWritePropertyCache<T> ReadWritePropertyCache { get; }
    }
}

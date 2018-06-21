namespace MassTransit.Internals.Reflection
{
    public interface IReadProperty<in T, out TProperty> :
        IReadProperty<T>
    {
        TProperty Get(T entity);
    }


    public interface IReadProperty<in T>
    {
        string Name { get; }
    }
}
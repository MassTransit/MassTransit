namespace MassTransit.Internals.Reflection
{
    public interface IWriteProperty<in T, in TProperty> :
        IWriteProperty<T>
    {
        void Set(T entity, TProperty value);
    }


    public interface IWriteProperty<in T>
    {
        string Name { get; }
    }
}
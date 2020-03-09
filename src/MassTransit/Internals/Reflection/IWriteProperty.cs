namespace MassTransit.Internals.Reflection
{
    public interface IWriteProperty<in T, in TProperty> :
        IWriteProperty<T>
        where T : class
    {
        void Set(T entity, TProperty value);
    }


    public interface IWriteProperty<in T>
        where T : class
    {
    }
}

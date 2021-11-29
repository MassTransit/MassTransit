namespace MassTransit.Internals
{
    using System.Reflection;


    public interface IWritePropertyCache<in T>
        where T : class
    {
        bool CanWrite(string name);

        IWriteProperty<T, TProperty> GetProperty<TProperty>(string name);
        IWriteProperty<T, TProperty> GetProperty<TProperty>(PropertyInfo propertyInfo);
    }
}

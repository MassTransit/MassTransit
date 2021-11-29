namespace MassTransit.Internals
{
    using System.Reflection;


    public interface IReadPropertyCache<in T>
        where T : class
    {
        IReadProperty<T, TProperty> GetProperty<TProperty>(string name);
        IReadProperty<T, TProperty> GetProperty<TProperty>(PropertyInfo propertyInfo);
    }
}

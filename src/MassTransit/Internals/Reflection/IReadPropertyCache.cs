namespace MassTransit.Internals.Reflection
{
    using System.Reflection;


    public interface IReadPropertyCache<in TEntity>
    {
        IReadProperty<TEntity, TProperty> GetProperty<TProperty>(string name);
        IReadProperty<TEntity, TProperty> GetProperty<TProperty>(PropertyInfo propertyInfo);
    }
}
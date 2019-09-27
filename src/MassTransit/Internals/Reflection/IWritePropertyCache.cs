namespace MassTransit.Internals.Reflection
{
    using System.Reflection;


    public interface IWritePropertyCache<in TEntity>
    {
        IWriteProperty<TEntity, TProperty> GetProperty<TProperty>(string name);
        IWriteProperty<TEntity, TProperty> GetProperty<TProperty>(PropertyInfo propertyInfo);
    }
}

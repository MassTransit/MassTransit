namespace MassTransit.Internals.Reflection
{
    public interface IWritePropertyCache<in TEntity>
    {
        IWriteProperty<TEntity, TProperty> GetProperty<TProperty>(string name);
    }
}
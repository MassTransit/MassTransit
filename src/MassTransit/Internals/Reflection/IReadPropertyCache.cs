namespace MassTransit.Internals.Reflection
{
    public interface IReadPropertyCache<in TEntity>
    {
        IReadProperty<TEntity, TProperty> GetProperty<TProperty>(string name);
    }
}
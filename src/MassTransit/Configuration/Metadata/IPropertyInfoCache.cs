namespace MassTransit.Metadata
{
    public interface IPropertyInfoCache
    {
        Contracts.PropertyInfo[] Properties { get; }
    }
}

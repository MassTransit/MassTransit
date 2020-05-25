namespace MassTransit.Metadata
{
    using Contracts.Metadata;


    public interface IPropertyInfoCache
    {
        PropertyInfo[] Properties { get; }
    }
}

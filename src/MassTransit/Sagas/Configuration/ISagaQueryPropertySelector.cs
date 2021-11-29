namespace MassTransit.Configuration
{
    public interface ISagaQueryPropertySelector<in TData, TProperty>
        where TData : class
    {
        bool TryGetProperty(ConsumeContext<TData> context, out TProperty property);
    }
}

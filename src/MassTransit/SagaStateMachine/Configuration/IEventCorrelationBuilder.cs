namespace MassTransit.Configuration
{
    public interface IEventCorrelationBuilder
    {
        EventCorrelation Build();
    }
}

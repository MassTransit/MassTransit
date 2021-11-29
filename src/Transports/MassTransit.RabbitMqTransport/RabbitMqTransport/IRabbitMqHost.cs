namespace MassTransit.RabbitMqTransport
{
    using Transports;


    public interface IRabbitMqHost :
        IHost<IRabbitMqReceiveEndpointConfigurator>
    {
    }
}

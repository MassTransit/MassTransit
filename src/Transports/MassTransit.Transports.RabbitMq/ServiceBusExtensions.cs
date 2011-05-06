namespace MassTransit.Transports.RabbitMq
{
    using System;

    public static class ServiceBusExtensions
    {
        public static IEndpoint GetEndpoint<TMessage>(this IServiceBus bus) where TMessage : class
        {
            return GetEndpoint(bus, typeof (TMessage));
        }

        public static IEndpoint GetEndpoint(this IServiceBus bus, Type type)
        {
            return null;
        }
    }
}
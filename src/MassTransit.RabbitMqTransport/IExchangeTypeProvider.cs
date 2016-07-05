using System;

namespace MassTransit.RabbitMqTransport
{
    /// <summary>
    /// Provides a type for an exchange based on the name of the exchange
    /// </summary>
    public interface IExchangeTypeProvider
    {
        string GetTypeForExchangeName(String exchangeName);
    }
}
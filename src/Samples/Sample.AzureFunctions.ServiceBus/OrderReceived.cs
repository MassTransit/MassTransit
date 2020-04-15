namespace Sample.AzureFunctions.ServiceBus
{
    using System;


    public interface OrderReceived
    {
        Guid OrderId { get; }
        DateTime Timestamp { get; }

        string OrderNumber { get; }
    }
}

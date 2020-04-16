namespace Sample.AzureFunctions.ServiceBus
{
    using System;


    public interface OrderAccepted
    {
        Guid OrderId { get; }
        string OrderNumber { get; }
    }
}

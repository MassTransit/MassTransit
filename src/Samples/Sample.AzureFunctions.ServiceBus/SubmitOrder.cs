namespace Sample.AzureFunctions.ServiceBus
{
    using System;


    public interface SubmitOrder
    {
        Guid OrderId { get; }
        string OrderNumber { get; }
    }
}

namespace MassTransit.TestComponents.Futures
{
    using System;


    public interface CalculatePrice
    {
        Guid OrderLineId { get; }

        string Sku { get; }
        string ContractNumber { get; }
    }
}

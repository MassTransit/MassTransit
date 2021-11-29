namespace MassTransit.TestFramework.Futures
{
    using System;


    public interface CalculatePrice
    {
        Guid OrderLineId { get; }

        string Sku { get; }
        string ContractNumber { get; }
    }
}

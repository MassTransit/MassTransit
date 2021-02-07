namespace MassTransit.TestComponents.Futures
{
    using System;


    public interface PriceCalculation
    {
        Guid OrderLineId { get; }

        decimal Amount { get; }
    }
}

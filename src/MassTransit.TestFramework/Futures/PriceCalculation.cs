namespace MassTransit.TestFramework.Futures
{
    using System;


    public interface PriceCalculation
    {
        Guid OrderLineId { get; }

        decimal Amount { get; }
    }
}

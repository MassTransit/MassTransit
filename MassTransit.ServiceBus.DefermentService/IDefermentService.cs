namespace MassTransit.ServiceBus.DefermentService
{
    using System;

    public interface IDefermentService
    {
        int Defer(object msg, TimeSpan amountOfTimeToDefer);
    }
}
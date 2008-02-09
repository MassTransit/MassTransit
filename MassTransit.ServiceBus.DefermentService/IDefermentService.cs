namespace MassTransit.ServiceBus.DefermentService
{
    using System;

    public interface IDefermentService
    {
        int Defer(IMessage msg, TimeSpan amountOfTimeToDefer);
    }
}
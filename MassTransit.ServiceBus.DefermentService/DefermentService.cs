namespace MassTransit.ServiceBus.DefermentService
{
    using System;

    public class DefermentService : IDefermentService
    {
        public int Defer(IMessage msg, TimeSpan amountOfTimeToDefer)
        {
            return 1;
        }
    }
}
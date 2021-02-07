namespace MassTransit.Futures
{
    using System;


    public delegate Uri RequestAddressProvider<in TMessage>(FutureConsumeContext<TMessage> context)
        where TMessage : class;
}

namespace MassTransit.Futures
{
    using System;


    public delegate Guid PendingIdProvider<in T>(T message)
        where T : class;
}

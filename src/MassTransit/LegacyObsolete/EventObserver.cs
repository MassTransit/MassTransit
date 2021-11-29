namespace MassTransit
{
}


namespace Automatonymous
{
    using System;
    using MassTransit;


    [Obsolete("Deprecated, use IEventObserver instead")]
    public interface EventObserver<TSaga> :
        IEventObserver<TSaga>
        where TSaga : class, ISaga
    {
    }
}

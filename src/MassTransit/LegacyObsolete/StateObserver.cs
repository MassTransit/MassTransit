namespace MassTransit
{
}


namespace Automatonymous
{
    using System;
    using MassTransit;


    [Obsolete("Deprecated, use IStateObserver instead")]
    public interface StateObserver<TSaga> :
        IStateObserver<TSaga>
        where TSaga : class, ISaga
    {
    }
}

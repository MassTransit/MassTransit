namespace MassTransit
{
}


namespace Automatonymous
{
    using System;
    using MassTransit;


    [Obsolete]
    public interface Activity :
        IVisitable
    {
    }


    /// <summary>
    /// An activity is part of a behavior that is executed in order
    /// </summary>
    /// <typeparam name="TSaga"></typeparam>
    [Obsolete("Deprecated, use IStateMachineActivity<TSaga> instead", true)]
    public interface Activity<TSaga> :
        IStateMachineActivity<TSaga>,
        Activity
        where TSaga : class, ISaga
    {
    }


    [Obsolete("Deprecated, use IStateMachineActivity<TSaga, TMessage> instead", true)]
    public interface Activity<TSaga, TMessage> :
        IStateMachineActivity<TSaga, TMessage>,
        Activity
        where TSaga : class, ISaga
        where TMessage : class
    {
    }
}

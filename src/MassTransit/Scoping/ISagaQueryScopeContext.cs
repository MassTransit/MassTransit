namespace MassTransit.Scoping
{
    using System;
    using Saga;


    public interface ISagaQueryScopeContext<TSaga, out T> :
        IDisposable
        where TSaga : class, ISaga
        where T : class
    {
        SagaQueryConsumeContext<TSaga, T> Context { get; }
    }
}
namespace MassTransit.Scoping
{
    using System;


    public interface IPublishScopeContext<out T> :
        IDisposable
        where T : class
    {
        PublishContext<T> Context { get; }
    }
}

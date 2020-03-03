namespace MassTransit
{
    using System;
    using GreenPipes;


    /// <summary>
    /// A request client factory which is unique to each consume, but uses a return path specified
    /// by the creation of the factory
    /// </summary>
    /// <typeparam name="TRequest"></typeparam>
    /// <typeparam name="TResponse"></typeparam>
    [Obsolete("This will be deprecated in the next release")]
    public interface IRequestClientFactory<TRequest, TResponse> :
        IAsyncDisposable
        where TRequest : class
        where TResponse : class
    {
        IRequestClient<TRequest, TResponse> CreateRequestClient(ConsumeContext consumeContext, TimeSpan? timeout = null,
            TimeSpan? timeToLive = null, Action<SendContext<TRequest>> callback = null);
    }
}

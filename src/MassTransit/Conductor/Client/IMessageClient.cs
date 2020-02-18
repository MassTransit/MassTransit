namespace MassTransit.Conductor.Client
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Clients;
    using GreenPipes;


    public interface IMessageClient<T> :
        IMessageClient
        where T : class
    {
        Task<IRequestSendEndpoint<T>> GetServiceSendEndpoint(ClientFactoryContext clientFactoryContext, T message, ConsumeContext consumeContext = default,
            CancellationToken cancellationToken = default);
    }


    public interface IMessageClient :
        IAsyncDisposable
    {
        Guid ClientId { get; }

        Type MessageType { get; }
    }
}

namespace MassTransit.Conductor.Client
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using GreenPipes;


    public interface IMessageClient<in T> :
        IMessageClient
        where T : class
    {
        Task<ISendEndpoint> GetServiceSendEndpoint(ISendEndpointProvider sendEndpointProvider, T message, CancellationToken cancellationToken = default);
    }


    public interface IMessageClient :
        IAsyncDisposable
    {
        Guid ClientId { get; }

        Type MessageType { get; }
    }
}

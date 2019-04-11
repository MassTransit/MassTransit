namespace MassTransit.Conductor
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;


    public interface IServiceClient :
        IClientFactory
    {
        Guid ClientId { get; }

        Task<ISendEndpoint> GetServiceSendEndpoint<T>(ISendEndpointProvider sendEndpointProvider, CancellationToken cancellationToken)
            where T : class;
    }
}

namespace MassTransit.Conductor.Client
{
    using System;
    using System.Threading.Tasks;


    public interface IMessageClient<T> :
        IMessageClient
        where T : class
    {
    }


    public interface IMessageClient
    {
        Type MessageType { get; }

        Task<ISendEndpoint> GetServiceSendEndpoint(ISendEndpointProvider sendEndpointProvider);
    }
}

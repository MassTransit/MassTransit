namespace MassTransit.Conductor.Server
{
    using System;
    using System.Threading.Tasks;
    using Contexts;
    using Contracts;


    public interface IMessageEndpoint<TMessage> :
        IMessageEndpoint
        where TMessage : class
    {
        /// <summary>
        /// Link a client to the message endpoint
        /// </summary>
        /// <param name="clientId"></param>
        /// <param name="clientAddress"></param>
        /// <returns></returns>
        Task Link(Guid clientId, Uri clientAddress);

        /// <summary>
        /// Unlink a client, removing the entry from the cache
        /// </summary>
        /// <param name="clientId"></param>
        /// <returns></returns>
        Task Unlink(Guid clientId);

        /// <summary>
        /// Accept (or rejected) a request, and return the appropriate proxy context
        /// </summary>
        /// <param name="clientId"></param>
        /// <param name="requestId"></param>
        /// <returns></returns>
        Task<RequestClientContext> Accept(Guid clientId, Guid requestId);

        /// <summary>
        /// Notify all clients with the message
        /// </summary>
        /// <param name="sendEndpointProvider"></param>
        /// <param name="message"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        Task NotifyClients<T>(ISendEndpointProvider sendEndpointProvider, T message);
    }


    public interface IMessageEndpoint
    {
        Uri ServiceAddress { get; }

        EndpointInfo EndpointInfo { get; }

        Task NotifyUp(IServiceInstance instance);

        Task NotifyDown(IServiceInstance instance);
    }
}

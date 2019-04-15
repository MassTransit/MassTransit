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
        /// Accept (or rejected) a request, and return the appropriate proxy context
        /// </summary>
        /// <param name="clientId"></param>
        /// <param name="requestId"></param>
        /// <returns></returns>
        Task<RequestClientContext> Accept(Guid clientId, Guid requestId);
    }


    public interface IMessageEndpoint
    {
        Uri ServiceAddress { get; }

        EndpointInfo EndpointInfo { get; }

        Task NotifyUp(IServiceInstance instance);

        Task NotifyDown(IServiceInstance instance);
    }
}

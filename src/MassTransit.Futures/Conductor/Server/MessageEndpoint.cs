namespace MassTransit.Conductor.Server
{
    using System;
    using System.Threading.Tasks;
    using Contexts;
    using Contracts;


    public class MessageEndpoint<TMessage> :
        IMessageEndpoint<TMessage>
        where TMessage : class
    {
        readonly IServiceEndpoint _endpoint;

        public MessageEndpoint(IServiceEndpoint endpoint)
        {
            _endpoint = endpoint;
        }

        public Task<RequestClientContext> Accept(Guid clientId, Guid requestId)
        {
            var context = new ConductorRequestClientContext(clientId, requestId);

            return Task.FromResult<RequestClientContext>(context);
        }

        public Uri ServiceAddress => _endpoint.ServiceAddress;

        public EndpointInfo EndpointInfo => _endpoint.EndpointInfo;

        public Task NotifyUp(IServiceInstance instance)
        {
            return instance.NotifyUp(this);
        }

        public Task NotifyDown(IServiceInstance instance)
        {
            return instance.NotifyDown(this);
        }
    }
}

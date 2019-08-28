namespace MassTransit.Conductor.Server
{
    using System;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Contexts;
    using Contracts;
    using GreenPipes.Caching;
    using Util;


    public class MessageEndpoint<TMessage> :
        IMessageEndpoint<TMessage>
        where TMessage : class
    {
        readonly IServiceEndpoint _endpoint;
        readonly ICache<ClientInfo> _cache;
        readonly IIndex<Guid, ClientInfo> _index;

        public MessageEndpoint(IServiceEndpoint endpoint)
        {
            _endpoint = endpoint;

            var cacheSettings = new CacheSettings(10000, TimeSpan.FromSeconds(1), TimeSpan.FromMinutes(30));
            _cache = new GreenCache<ClientInfo>(cacheSettings);
            _index = _cache.AddIndex("clientId", x => x.ClientId);
        }

        public Task Link(Guid clientId, Uri clientAddress)
        {
            return _index.Get(clientId, id => Task.FromResult(new ClientInfo(clientId, clientAddress)));
        }

        public Task Unlink(Guid clientId)
        {
            _index.Remove(clientId);

            return TaskUtil.Completed;
        }

        public Task<RequestClientContext> Accept(Guid clientId, Guid requestId)
        {
            var context = new ConductorRequestClientContext(clientId, requestId);

            return Task.FromResult<RequestClientContext>(context);
        }

        public Task NotifyClients<T>(ISendEndpointProvider sendEndpointProvider, T message)
        {
            async Task NotifyClient(Task<ClientInfo> x)
            {
                try
                {
                    var clientInfo = await x.ConfigureAwait(false);

                    var endpoint = await sendEndpointProvider.GetSendEndpoint(clientInfo.ClientAddress).ConfigureAwait(false);

                    await endpoint.Send(message).ConfigureAwait(false);
                }
                catch
                {
                }
            }

            return Task.WhenAll(_cache.GetAll().Select(NotifyClient));
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


        class ClientInfo
        {
            int _remaining;

            public ClientInfo(Guid clientId, Uri clientAddress)
            {
                ClientId = clientId;
                ClientAddress = clientAddress;
            }

            public Uri ClientAddress { get; }
            public Guid ClientId { get; }

            public int Remaining => _remaining;

            public void Add(int count)
            {
                Interlocked.Add(ref _remaining, count);
            }

            public (bool accepted, int remaining) Accept()
            {
                var remaining = Interlocked.Decrement(ref _remaining);

                return (remaining >= 0, Math.Max(0, remaining));
            }
        }
    }
}

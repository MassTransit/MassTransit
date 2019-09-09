namespace MassTransit.Tests
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Context;
    using Context.Converters;
    using Contracts;
    using GreenPipes;
    using GreenPipes.Caching;
    using Initializers;
    using NUnit.Framework;
    using Subjects;
    using TestFramework;
    using Util;


    [TestFixture]
    public class ServiceEndpoint_Specs :
        InMemoryTestFixture
    {
        [Test]
        public async Task Should_do_the_thing()
        {
            var upHandler = SubscribeHandler<Up<DeployPayload>>();

            var clientId = NewId.NextGuid();

            await Bus.Publish<Link<DeployPayload>>(new {ClientId = clientId}, context => context.ResponseAddress = Bus.Address);

            var up = await upHandler;

            var serviceEndpoint = await Bus.GetSendEndpoint(up.Message.ServiceAddress);

            var acceptHandler = SubscribeHandler<Accept<DeployPayload>>();

            await serviceEndpoint.Send<Ask<DeployPayload>>(new
                {
                    ClientId = clientId,
                    up.Message.ServiceAddress
                },
                context => context.ResponseAddress = Bus.Address);

            var accept = await acceptHandler;

            var client = Bus.CreateRequestClient<DeployPayload>(accept.Message.Endpoint.EndpointAddress);
            var request = client.Create(new {Target = "Soon Dead"});
            request.UseExecute(context => context.Headers.Set(MessageHeaders.ClientId, clientId.ToString("N")));

            Response<PayloadDeployed> response = await request.GetResponse<PayloadDeployed>();

            int? remaining = response.Headers.Get(MessageHeaders.Request.Remaining, default(int?));

            Assert.That(remaining.HasValue);
            Assert.That(remaining.Value, Is.EqualTo(0));
        }

        protected override void ConfigureInMemoryBus(IInMemoryBusFactoryConfigurator configurator)
        {
            var endpointId = NewId.NextGuid();
            DateTime started = DateTime.UtcNow;

            configurator.ReceiveEndpoint(new TemporaryEndpointDefinition(), e =>
            {
                var endpointAddress = e.InputAddress;

                configurator.ReceiveEndpoint("service", s =>
                {
                    var serviceAddress = s.InputAddress;

                    configurator.ReceiveEndpoint(new ControlEndpointDefinition(), c =>
                    {
                        var controlAddress = c.InputAddress;

                        var settings = new ServiceEndpointSettings()
                        {
                            EndpointId = endpointId,
                            Started = started,
                            ServiceAddress = serviceAddress,
                            EndpointAddress = endpointAddress,
                            InstanceAddress = controlAddress,
                        };

                        var runtime = new EndpointRuntime(settings);

                        e.Consumer<DeployPayloadConsumer>(cfg =>
                        {
                            cfg.Message<DeployPayload>(m => m.UseFilter(new RequestLimitFilter<DeployPayload>(runtime)));
                        });

                        s.Consumer(() => new AskConsumer(settings, runtime));

                        c.Consumer(() => new LinkConsumer(settings, runtime));
                    });
                });
            });
        }


        public interface EndpointSettings :
            EndpointInfo
        {
            /// <summary>
            /// The service address shared by all service endpoints
            /// </summary>
            Uri ServiceAddress { get; }
        }


        class ServiceEndpointSettings :
            EndpointSettings
        {
            public Guid EndpointId { get; set; }
            public DateTime Started { get; set; }
            public Uri ServiceAddress { get; set; }
            public Uri EndpointAddress { get; set; }
            public Uri InstanceAddress { get; set; }
        }


        class LinkConsumer :
            IConsumer<Link<DeployPayload>>
        {
            readonly EndpointSettings _settings;
            readonly IEndpointRuntime _runtime;

            public LinkConsumer(EndpointSettings settings, IEndpointRuntime runtime)
            {
                _settings = settings;
                _runtime = runtime;
            }

            public async Task Consume(ConsumeContext<Link<DeployPayload>> context)
            {
                if (context.ResponseAddress == null)
                    return;

                await _runtime.Linked(context.Message.ClientId).ConfigureAwait(false);

                await context.RespondAsync<Up<DeployPayload>>(new
                {
                    _settings.ServiceAddress,
                    Endpoint = (EndpointInfo)_settings,
                });
            }
        }


        class ClientInfo
        {
            int _remaining;
            public Guid ClientId { get; set; }

            public int Remaining
            {
                get => _remaining;
            }

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


        public readonly struct Accepted
        {
            public readonly int Remaining;

            public Accepted(int remaining)
            {
                Remaining = remaining;
            }
        }


        public interface IEndpointRuntime
        {
            Task Linked(Guid clientId);

            Task<Accept<T>> Ask<T>(Guid clientId, int count)
                where T : class;

            Task<(bool accepted, int remaining)> Accept<T>(Guid clientId)
                where T : class;
        }


        class EndpointRuntime :
            IEndpointRuntime
        {
            readonly EndpointSettings _settings;
            readonly ICache<ClientInfo> _cache;
            readonly IIndex<Guid, ClientInfo> _index;

            public EndpointRuntime(EndpointSettings settings)
            {
                _settings = settings;

                var cacheSettings = new CacheSettings(1000, TimeSpan.FromSeconds(1), TimeSpan.FromHours(24));
                _cache = new GreenCache<ClientInfo>(cacheSettings);
                _index = _cache.AddIndex("clientId", x => x.ClientId);
            }

            static Task<ClientInfo> CreateClientInfo(Guid clientId)
            {
                return Task.FromResult(new ClientInfo() {ClientId = clientId});
            }

            public Task Linked(Guid clientId)
            {
                return _index.Get(clientId, CreateClientInfo);
            }

            public async Task<Accept<T>> Ask<T>(Guid clientId, int count)
                where T : class
            {
                var entry = await _index.Get(clientId, CreateClientInfo).ConfigureAwait(false);

                entry.Add(count);

                var context = await MessageInitializerCache<Accept<T>>.Initialize(new
                {
                    Endpoint = (EndpointInfo)_settings,
                    Count = count,
                }).ConfigureAwait(false);

                return context.Message;
            }

            public async Task<(bool accepted, int remaining)> Accept<T>(Guid clientId)
                where T : class
            {
                var entry = await _index.Get(clientId, CreateClientInfo).ConfigureAwait(false);

                return entry.Accept();
            }
        }


        class AskConsumer :
            IConsumer<Ask<DeployPayload>>
        {
            readonly EndpointSettings _settings;
            readonly IEndpointRuntime _runtime;

            public AskConsumer(EndpointSettings settings, IEndpointRuntime runtime)
            {
                _settings = settings;
                _runtime = runtime;
            }

            public async Task Consume(ConsumeContext<Ask<DeployPayload>> context)
            {
                if (context.ResponseAddress == null)
                    return;

                var accept = await _runtime.Ask<DeployPayload>(context.Message.ClientId, 1).ConfigureAwait(false);

                await context.RespondAsync(accept);
            }
        }


        class RequestLimitFilter<T> :
            IFilter<ConsumeContext<T>>
            where T : class
        {
            readonly IEndpointRuntime _runtime;

            public RequestLimitFilter(IEndpointRuntime runtime)
            {
                _runtime = runtime;
            }

            public async Task Send(ConsumeContext<T> context, IPipe<ConsumeContext<T>> next)
            {
                if (!context.RequestId.HasValue)
                    throw new RequestException("RequestId is required");

                var clientId = context.Headers.Get(MessageHeaders.ClientId, default(Guid?));
                if (!clientId.HasValue)
                    throw new RequestException($"ClientId not specified (requestId: {context.RequestId})");

                var (accepted, remaining) = await _runtime.Accept<T>(clientId.Value);
                if (!accepted)
                    throw new RequestException($"Request not accepted (clientId: {clientId}, requestId: {context.RequestId})");

                var clientContext = new RequestClientContextImpl(clientId.Value, context.RequestId.Value, remaining);

                context.AddOrUpdatePayload(() => clientContext, existing => clientContext);

                var proxy = new RequestConsumeContextProxy<T>(context, clientContext);

                await next.Send(proxy).ConfigureAwait(false);
            }

            public void Probe(ProbeContext context)
            {
                context.CreateFilterScope("consumerLimit");
            }
        }


        public interface RequestClientContext
        {
            Guid ClientId { get; }

            int? Remaining { get; }
            Guid RequestId { get; }
        }


        class RequestConsumeContextProxy<TMessage> :
            ConsumeContextProxy<TMessage>
            where TMessage : class
        {
            readonly RequestClientContext _clientContext;

            public RequestConsumeContextProxy(ConsumeContext<TMessage> context, RequestClientContext clientContext)
                : base(context)
            {
                _clientContext = clientContext;
            }

            public override async Task<ISendEndpoint> GetSendEndpoint(Uri address)
            {
                var endpoint = await base.GetSendEndpoint(address).ConfigureAwait(false);

                return new RequestSendEndpoint(endpoint, _clientContext);
            }
        }


        class RequestClientContextImpl :
            RequestClientContext
        {
            public RequestClientContextImpl(Guid clientId, Guid requestId, int? remaining)
            {
                ClientId = clientId;
                Remaining = remaining;
                RequestId = requestId;
            }

            public Guid ClientId { get; }
            public int? Remaining { get; }
            public Guid RequestId { get; }
        }


        class DeployPayloadConsumer :
            IConsumer<DeployPayload>
        {
            public async Task Consume(ConsumeContext<DeployPayload> context)
            {
                LogContext.Info?.Log("Deploying Payload: {Target}", context.Message.Target);

                await context.RespondAsync<PayloadDeployed>(new { });
            }
        }
    }


    public struct RequestSendContextPipe<T> :
        IPipe<SendContext<T>>
        where T : class
    {
        readonly ServiceEndpoint_Specs.RequestClientContext _clientContext;
        readonly IPipe<SendContext<T>> _pipe;

        public RequestSendContextPipe(ServiceEndpoint_Specs.RequestClientContext clientContext)
        {
            _clientContext = clientContext;

            _pipe = default;
        }

        public RequestSendContextPipe(ServiceEndpoint_Specs.RequestClientContext clientContext, IPipe<SendContext<T>> pipe)
        {
            _clientContext = clientContext;
            _pipe = pipe;
        }

        public RequestSendContextPipe(ServiceEndpoint_Specs.RequestClientContext clientContext, IPipe<SendContext> pipe)
        {
            _clientContext = clientContext;
            _pipe = pipe;
        }

        void IProbeSite.Probe(ProbeContext context)
        {
            _pipe?.Probe(context);
        }

        public Task Send(SendContext<T> context)
        {
            context.Headers.Set(MessageHeaders.ClientId, _clientContext.ClientId.ToString("N"));
            context.Headers.Set(MessageHeaders.Request.Remaining, _clientContext.Remaining.Value.ToString());

            return _pipe.IsNotEmpty()
                ? _pipe.Send(context)
                : TaskUtil.Completed;
        }
    }


    /// <summary>
    /// Request Endpoint Context
    /// </summary>
    public class RequestSendEndpoint :
        ISendEndpoint
    {
        readonly ServiceEndpoint_Specs.RequestClientContext _clientContext;
        readonly ISendEndpoint _endpoint;

        public RequestSendEndpoint(ISendEndpoint endpoint, ServiceEndpoint_Specs.RequestClientContext clientContext)
        {
            _endpoint = endpoint;
            _clientContext = clientContext;
        }

        public ConnectHandle ConnectSendObserver(ISendObserver observer)
        {
            return _endpoint.ConnectSendObserver(observer);
        }

        public Task Send<T>(T message, CancellationToken cancellationToken)
            where T : class
        {
            if (message == null)
                throw new ArgumentNullException(nameof(message));

            var sendContextPipe = new RequestSendContextPipe<T>(_clientContext);

            return _endpoint.Send(message, sendContextPipe, cancellationToken);
        }

        public Task Send<T>(T message, IPipe<SendContext<T>> pipe, CancellationToken cancellationToken)
            where T : class
        {
            if (message == null)
                throw new ArgumentNullException(nameof(message));

            if (pipe == null)
                throw new ArgumentNullException(nameof(pipe));

            var sendContextPipe = new RequestSendContextPipe<T>(_clientContext, pipe);

            return _endpoint.Send(message, sendContextPipe, cancellationToken);
        }

        public Task Send(object message, CancellationToken cancellationToken)
        {
            if (message == null)
                throw new ArgumentNullException(nameof(message));

            var messageType = message.GetType();

            return SendEndpointConverterCache.Send(this, message, messageType, cancellationToken);
        }

        public Task Send(object message, Type messageType, CancellationToken cancellationToken)
        {
            if (message == null)
                throw new ArgumentNullException(nameof(message));

            if (messageType == null)
                throw new ArgumentNullException(nameof(messageType));

            return SendEndpointConverterCache.Send(this, message, messageType, cancellationToken);
        }

        public Task Send<T>(object values, CancellationToken cancellationToken)
            where T : class
        {
            if (values == null)
                throw new ArgumentNullException(nameof(values));

            return MessageInitializerCache<T>.Send(this, values, cancellationToken);
        }

        public Task Send<T>(T message, IPipe<SendContext> pipe, CancellationToken cancellationToken)
            where T : class
        {
            if (message == null)
                throw new ArgumentNullException(nameof(message));

            if (pipe == null)
                throw new ArgumentNullException(nameof(pipe));

            var sendContextPipe = new RequestSendContextPipe<T>(_clientContext, pipe);

            return _endpoint.Send(message, sendContextPipe, cancellationToken);
        }

        public Task Send(object message, IPipe<SendContext> pipe, CancellationToken cancellationToken)
        {
            if (message == null)
                throw new ArgumentNullException(nameof(message));

            if (pipe == null)
                throw new ArgumentNullException(nameof(pipe));

            var messageType = message.GetType();

            return SendEndpointConverterCache.Send(this, message, messageType, pipe, cancellationToken);
        }

        public Task Send(object message, Type messageType, IPipe<SendContext> pipe, CancellationToken cancellationToken)
        {
            if (message == null)
                throw new ArgumentNullException(nameof(message));

            if (messageType == null)
                throw new ArgumentNullException(nameof(messageType));

            if (pipe == null)
                throw new ArgumentNullException(nameof(pipe));

            return SendEndpointConverterCache.Send(this, message, messageType, pipe, cancellationToken);
        }

        public Task Send<T>(object values, IPipe<SendContext<T>> pipe, CancellationToken cancellationToken)
            where T : class
        {
            if (values == null)
                throw new ArgumentNullException(nameof(values));

            return MessageInitializerCache<T>.Send(this, values, pipe, cancellationToken);
        }

        public Task Send<T>(object values, IPipe<SendContext> pipe, CancellationToken cancellationToken)
            where T : class
        {
            if (values == null)
                throw new ArgumentNullException(nameof(values));

            if (pipe == null)
                throw new ArgumentNullException(nameof(pipe));

            return MessageInitializerCache<T>.Send(this, values, pipe, cancellationToken);
        }
    }


    namespace Subjects
    {
        public interface DeployPayload
        {
            string Target { get; }
        }


        public interface PayloadDeployed
        {
        }
    }
}

namespace MassTransit.WebJobs.EventHubsIntegration
{
    using System;
    using System.Collections.Concurrent;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Azure.EventHubs;
    using Registration;
    using Saga;
    using ServiceBusIntegration;
    using Transport;


    public class EventReceiver :
        IEventReceiver
    {
        readonly IAsyncBusHandle _busHandle;
        readonly IBusConnector _busConnector;
        readonly IRegistration _registration;
        readonly ConcurrentDictionary<string, IEventDataReceiver> _receivers;

        public EventReceiver(IRegistration registration, IAsyncBusHandle busHandle, IBusConnector busConnector)
        {
            _registration = registration;
            _busHandle = busHandle;
            _busConnector = busConnector;

            _receivers = new ConcurrentDictionary<string, IEventDataReceiver>();
        }

        public Task Handle(string entityName, EventData message, CancellationToken cancellationToken)
        {
            var receiver = CreateBrokeredMessageReceiver(entityName, cfg =>
            {
                cfg.ConfigureConsumers(_registration);
                cfg.ConfigureSagas(_registration);
            });

            return receiver.Handle(message, cancellationToken);
        }

        public Task HandleConsumer<TConsumer>(string entityName, EventData message, CancellationToken cancellationToken)
            where TConsumer : class, IConsumer
        {
            var receiver = CreateBrokeredMessageReceiver(entityName, cfg =>
            {
                cfg.ConfigureConsumer<TConsumer>(_registration);
            });

            return receiver.Handle(message, cancellationToken);
        }

        public Task HandleSaga<TSaga>(string entityName, EventData message, CancellationToken cancellationToken)
            where TSaga : class, ISaga
        {
            var receiver = CreateBrokeredMessageReceiver(entityName, cfg =>
            {
                cfg.ConfigureSaga<TSaga>(_registration);
            });

            return receiver.Handle(message, cancellationToken);
        }

        public Task HandleExecuteActivity<TActivity>(string entityName, EventData message, CancellationToken cancellationToken)
            where TActivity : class
        {
            var receiver = CreateBrokeredMessageReceiver(entityName, cfg =>
            {
                cfg.ConfigureExecuteActivity(_registration, typeof(TActivity));
            });

            return receiver.Handle(message, cancellationToken);
        }

        IEventDataReceiver CreateBrokeredMessageReceiver(string entityName, Action<IReceiveEndpointConfigurator> configure)
        {
            if (string.IsNullOrWhiteSpace(entityName))
                throw new ArgumentNullException(nameof(entityName));
            if (configure == null)
                throw new ArgumentNullException(nameof(configure));

            return _receivers.GetOrAdd(entityName, name =>
            {
                var receiveEndpointContext = _busConnector.CreateReceiveEndpointContext(entityName, configure);
                return new EventDataReceiver(receiveEndpointContext);
            });
        }

        public void Dispose()
        {
        }
    }
}

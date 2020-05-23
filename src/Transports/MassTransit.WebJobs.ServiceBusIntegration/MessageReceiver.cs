namespace MassTransit.WebJobs.ServiceBusIntegration
{
    using System;
    using System.Collections.Concurrent;
    using System.Threading;
    using System.Threading.Tasks;
    using Azure.ServiceBus.Core.Configuration;
    using Azure.ServiceBus.Core.Transport;
    using Microsoft.Azure.ServiceBus;
    using Registration;
    using Saga;


    public class MessageReceiver :
        IMessageReceiver
    {
        readonly IAsyncBusHandle _busHandle;
        readonly IServiceBusHostConfiguration _hostConfiguration;
        readonly ConcurrentDictionary<string, IBrokeredMessageReceiver> _receivers;
        readonly IRegistration _registration;

        public MessageReceiver(IRegistration registration, IAsyncBusHandle busHandle, IBusInstance busInstance)
        {
            if (busInstance.HostConfiguration == null)
                throw new ArgumentNullException(nameof(busInstance), "The bus instance was not created properly, the hostConfiguration was not present.");

            _hostConfiguration = busInstance.HostConfiguration as IServiceBusHostConfiguration;
            if (_hostConfiguration == null)
                throw new ArgumentException("The hostConfiguration was not configured for Azure Service Bus", nameof(busInstance));

            _registration = registration;
            _busHandle = busHandle;

            _receivers = new ConcurrentDictionary<string, IBrokeredMessageReceiver>();
        }

        public Task Handle(string entityName, Message message, CancellationToken cancellationToken)
        {
            var receiver = CreateBrokeredMessageReceiver(entityName, cfg =>
            {
                cfg.ConfigureConsumers(_registration);
                cfg.ConfigureSagas(_registration);
            });

            return receiver.Handle(message, cancellationToken);
        }

        public Task HandleConsumer<TConsumer>(string entityName, Message message, CancellationToken cancellationToken)
            where TConsumer : class, IConsumer
        {
            var receiver = CreateBrokeredMessageReceiver(entityName, cfg =>
            {
                cfg.ConfigureConsumer<TConsumer>(_registration);
            });

            return receiver.Handle(message, cancellationToken);
        }

        public Task HandleSaga<TSaga>(string entityName, Message message, CancellationToken cancellationToken)
            where TSaga : class, ISaga
        {
            var receiver = CreateBrokeredMessageReceiver(entityName, cfg =>
            {
                cfg.ConfigureSaga<TSaga>(_registration);
            });

            return receiver.Handle(message, cancellationToken);
        }

        public Task HandleExecuteActivity<TActivity>(string entityName, Message message, CancellationToken cancellationToken)
            where TActivity : class
        {
            var receiver = CreateBrokeredMessageReceiver(entityName, cfg =>
            {
                cfg.ConfigureExecuteActivity(_registration, typeof(TActivity));
            });

            return receiver.Handle(message, cancellationToken);
        }

        public void Dispose()
        {
        }

        IBrokeredMessageReceiver CreateBrokeredMessageReceiver(string entityName, Action<IReceiveEndpointConfigurator> configure)
        {
            if (string.IsNullOrWhiteSpace(entityName))
                throw new ArgumentNullException(nameof(entityName));
            if (configure == null)
                throw new ArgumentNullException(nameof(configure));

            return _receivers.GetOrAdd(entityName, name =>
            {
                var endpointConfiguration = _hostConfiguration.CreateReceiveEndpointConfiguration(entityName);

                var configurator = new BrokeredMessageReceiverConfiguration(_hostConfiguration, endpointConfiguration);

                configure(configurator);

                return configurator.Build();
            });
        }
    }
}

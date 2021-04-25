using System;
using System.Collections.Generic;
using EventStore.Client;
using GreenPipes;
using GreenPipes.Configurators;
using MassTransit.Configuration;
using MassTransit.EventStoreDbIntegration.Contexts;
using MassTransit.EventStoreDbIntegration.Filters;
using MassTransit.EventStoreDbIntegration.Serializers;
using MassTransit.Registration;
using MassTransit.Transports;

namespace MassTransit.EventStoreDbIntegration.Configurators
{
    public sealed class EventStoreDbCatchupSubscriptionConfigurator :
        ReceiverConfiguration,
        IEventStoreDbCatchupSubscriptionConfigurator,
        SubscriptionSettings
    {
        readonly IBusInstance _busInstance;
        readonly IReceiveEndpointConfiguration _endpointConfiguration;
        readonly IEventStoreDbHostConfiguration _hostConfiguration;
        readonly PipeConfigurator<SubscriptionContext> _processorConfigurator;
        IHeadersDeserializer _headersDeserializer;
        IEventFilter _allStreamEventFilter;
        UserCredentials _userCredentials;
        CheckpointStoreFactory _checkpointStoreFactory;
        bool _isCheckpointStoreConfigured = false;

        public EventStoreDbCatchupSubscriptionConfigurator(IEventStoreDbHostConfiguration hostConfiguration, StreamName streamName, string subscriptionName,
            IBusInstance busInstance, IReceiveEndpointConfiguration endpointConfiguration, IHeadersDeserializer headersDeserializer)
            : base(endpointConfiguration)
        {
            StreamName = streamName;
            SubscriptionName = subscriptionName;
            _hostConfiguration = hostConfiguration;
            _busInstance = busInstance;
            _endpointConfiguration = endpointConfiguration;

            CheckpointInterval = TimeSpan.FromMinutes(1);
            CheckpointMessageCount = 5000;
            ConcurrencyLimit = 1;

            //Actual checkpoint interval is AllStreamCheckpointInterval * maxSearchWindow (320 * 32 = 10,240)
            AllStreamCheckpointInterval = 320;

            HeadersDeserializer = headersDeserializer;

            _processorConfigurator = new PipeConfigurator<SubscriptionContext>();
        }

        public bool IsCatchupSubscription => true;

        public TimeSpan CheckpointInterval { get; set; }

        public ushort CheckpointMessageCount { get; set; }

        public int ConcurrencyLimit { get; }

        public IEventFilter AllStreamEventFilter
        {
            get => _allStreamEventFilter;
            set
            {
                if (!StreamName.IsAllStream)
                    throw new InvalidOperationException($"The {nameof(AllStreamEventFilter)} can only be set for an AllStream subscription.");

                _allStreamEventFilter = value ?? throw new ArgumentNullException(nameof(value));
            }
        }

        public uint AllStreamCheckpointInterval { get; set; }

        public UserCredentials UserCredentials
        {
            get => _userCredentials;
            set => _userCredentials = value ?? throw new ArgumentNullException(nameof(value));
        }

        public StreamName StreamName { get; }

        public string SubscriptionName { get; }

        public IHeadersDeserializer HeadersDeserializer
        {
            get => _headersDeserializer;
            set => _headersDeserializer = value ?? throw new ArgumentNullException(nameof(value));
        }

        public void SetCheckpointStore(CheckpointStoreFactory checkpointStoreFactory)
        {
            ThrowIfCheckpointStoreIsAlreadyConfigured();

            _checkpointStoreFactory = checkpointStoreFactory ?? throw new ArgumentNullException(nameof(checkpointStoreFactory));
        }

        public override IEnumerable<ValidationResult> Validate()
        {
            if (_headersDeserializer == null)
                yield return this.Failure("HeadersDeserializer", "should not be null");

            if (_checkpointStoreFactory == null)
                yield return this.Failure("CheckpointStoreFactory", "should not be null");

            foreach (var result in base.Validate())
                yield return result;
        }

        public ReceiveEndpoint Build()
        {
            IEventStoreDbSubscriptionContext CreateContext()
            {
                var builder = new EventStoreDbReceiveEndpointBuilder(_hostConfiguration, _busInstance, _endpointConfiguration, this, _headersDeserializer,
                    _checkpointStoreFactory);

                foreach (var specification in Specifications)
                    specification.Configure(builder);

                return builder.CreateReceiveEndpointContext();
            }

            var context = CreateContext();

            _processorConfigurator.UseFilter(new EventStoreDbSubscriptionFilter(context));

            IPipe<SubscriptionContext> processorPipe = _processorConfigurator.Build();

            var transport = new ReceiveTransport<SubscriptionContext>(_busInstance.HostConfiguration, context, () => context.ContextSupervisor,
                processorPipe);

            return new ReceiveEndpoint(transport, context);
        }

        void ThrowIfCheckpointStoreIsAlreadyConfigured()
        {
            if (_isCheckpointStoreConfigured)
                throw new ConfigurationException("CheckpointStore settings may not be specified more than once per catch-up subscription.");

            _isCheckpointStoreConfigured = true;
        }
    }
}

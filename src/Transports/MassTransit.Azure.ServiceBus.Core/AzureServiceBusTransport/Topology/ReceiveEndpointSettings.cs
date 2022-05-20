namespace MassTransit.AzureServiceBusTransport.Topology
{
    using System;
    using System.Collections.Generic;
    using Azure.Messaging.ServiceBus.Administration;
    using Configuration;


    public class ReceiveEndpointSettings :
        BaseClientSettings,
        ReceiveSettings
    {
        readonly ServiceBusQueueConfigurator _queueConfigurator;

        public ReceiveEndpointSettings(IServiceBusEndpointConfiguration endpointConfiguration, string queueName, ServiceBusQueueConfigurator queueConfigurator)
            : base(endpointConfiguration, queueConfigurator)
        {
            _queueConfigurator = queueConfigurator;

            Name = queueName;
        }

        public IServiceBusQueueConfigurator QueueConfigurator => _queueConfigurator;

        public override bool RequiresSession => _queueConfigurator.RequiresSession ?? false;
        public override int MaxConcurrentCallsPerSession => _queueConfigurator.MaxConcurrentCallsPerSession ?? 1;

        public bool RemoveSubscriptions { get; set; }

        public override string Path => _queueConfigurator.FullPath;

        public CreateQueueOptions GetCreateQueueOptions()
        {
            return _queueConfigurator.GetCreateQueueOptions();
        }

        public override void SelectBasicTier()
        {
            _queueConfigurator.AutoDeleteOnIdle = default;
            _queueConfigurator.DefaultMessageTimeToLive = Defaults.BasicMessageTimeToLive;
        }

        protected override IEnumerable<string> GetQueryStringOptions()
        {
            if (_queueConfigurator.AutoDeleteOnIdle.HasValue && _queueConfigurator.AutoDeleteOnIdle.Value > TimeSpan.Zero
                && _queueConfigurator.AutoDeleteOnIdle.Value != Defaults.AutoDeleteOnIdle)
                yield return $"autodelete={_queueConfigurator.AutoDeleteOnIdle.Value.TotalSeconds}";
        }
    }
}

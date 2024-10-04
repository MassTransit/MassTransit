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

        public bool RemoveSubscriptions { get; set; }
        public override int MaxConcurrentSessions => _queueConfigurator.MaxConcurrentSessions ?? Defaults.MaxConcurrentSessions;
        public override int MaxConcurrentCallsPerSession => _queueConfigurator.MaxConcurrentCallsPerSession ?? Defaults.MaxConcurrentCallsPerSessions;

        public override string Path => _queueConfigurator.FullPath;

        public CreateQueueOptions GetCreateQueueOptions()
        {
            return _queueConfigurator.GetCreateQueueOptions();
        }

        protected override IEnumerable<string> GetQueryStringOptions()
        {
            if (_queueConfigurator.AutoDeleteOnIdle.HasValue && _queueConfigurator.AutoDeleteOnIdle.Value > TimeSpan.Zero
                && _queueConfigurator.AutoDeleteOnIdle.Value != Defaults.AutoDeleteOnIdle)
                yield return $"autodelete={_queueConfigurator.AutoDeleteOnIdle.Value.TotalSeconds}";
        }
    }
}

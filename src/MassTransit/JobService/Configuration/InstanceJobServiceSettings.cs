#nullable enable
namespace MassTransit.Configuration
{
    using System;
    using System.Collections.Generic;
    using JobService;
    using Microsoft.Extensions.Options;


    public class InstanceJobServiceSettings :
        JobServiceSettings
    {
        readonly List<Action<IReceiveEndpointConfigurator>> _configureActions;

        readonly JobConsumerOptions _options;

        public InstanceJobServiceSettings(IOptions<JobConsumerOptions> options)
            : this(options.Value)
        {
        }

        public InstanceJobServiceSettings(JobConsumerOptions options)
        {
            _options = options;
            _configureActions = new List<Action<IReceiveEndpointConfigurator>>();

            JobService = new JobService(this);
        }

        public TimeSpan HeartbeatInterval => _options.HeartbeatInterval;

        public Uri? InstanceAddress { get; set; }
        public IReceiveEndpointConfigurator? InstanceEndpointConfigurator { get; set; }
        public IJobService JobService { get;  }

        public void AddConfigureAction(Action<IReceiveEndpointConfigurator>? configure)
        {
            if (configure != null)
                _configureActions.Add(configure);
        }

        public void ApplyConfiguration<T>(T configurator)
            where T : IReceiveEndpointConfigurator
        {
            InstanceEndpointConfigurator = configurator;

            for (var i = 0; i < _configureActions.Count; i++)
                _configureActions[i](configurator);

            InstanceAddress = configurator.InputAddress;
        }
    }
}

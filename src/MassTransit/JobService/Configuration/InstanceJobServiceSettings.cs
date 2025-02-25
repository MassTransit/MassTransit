#nullable enable
namespace MassTransit.Configuration
{
    using System;
    using JobService;
    using Microsoft.Extensions.Options;


    public class InstanceJobServiceSettings :
        JobServiceSettings
    {
        readonly JobConsumerOptions _options;

        public InstanceJobServiceSettings(IOptions<JobConsumerOptions> options)
            : this(options.Value)
        {
        }

        public InstanceJobServiceSettings(JobConsumerOptions options)
        {
            _options = options;

            JobService = new JobService(this);
        }

        public TimeSpan HeartbeatInterval => _options.HeartbeatInterval;

        public Uri? InstanceAddress { get; set; }
        public IReceiveEndpointConfigurator? InstanceEndpointConfigurator { get; set; }
        public IJobService JobService { get; }

        public void ApplyConfiguration<T>(T configurator)
            where T : IReceiveEndpointConfigurator
        {
            InstanceEndpointConfigurator = configurator;

            InstanceAddress = configurator.InputAddress;
        }
    }
}

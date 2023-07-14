#nullable enable
namespace MassTransit.DependencyInjection.Registration
{
    using System;
    using System.Collections.Generic;
    using Configuration;
    using Internals;
    using JobService;
    using NewIdFormatters;


    public class JobServiceRegistration :
        IJobServiceRegistration
    {
        readonly List<Action<JobConsumerOptions>> _configureActions;
        readonly List<IReceiveEndpointConfigurator> _dependencies;
        readonly EndpointRegistrationConfigurator<JobService> _endpointConfigurator;
        readonly Lazy<InstanceJobServiceSettings> _settings;

        public JobServiceRegistration()
        {
            _configureActions = new List<Action<JobConsumerOptions>>();
            _dependencies = new List<IReceiveEndpointConfigurator>(4);

            _settings = new Lazy<InstanceJobServiceSettings>(GetJobServiceSettings);

            _endpointConfigurator = new EndpointRegistrationConfigurator<JobService>
            {
                InstanceId = NewId.Next().ToString(ZBase32Formatter.LowerCase),
                Temporary = true
            };

            IncludeInConfigureEndpoints = !Type.HasAttribute<ExcludeFromConfigureEndpointsAttribute>();
        }

        JobServiceSettings Settings => _settings.Value;

        public Type Type => typeof(JobService);

        public bool IncludeInConfigureEndpoints { get; set; }

        public IEndpointRegistrationConfigurator EndpointRegistrationConfigurator => _endpointConfigurator;
        public IEndpointDefinition EndpointDefinition => new JobServiceEndpointDefinition(_endpointConfigurator.Settings, _settings.Value);

        public void AddConfigureAction(Action<JobConsumerOptions>? configure)
        {
            if (_settings.IsValueCreated)
                throw new ConfigurationException("The settings were already computed");

            if (configure != null)
                _configureActions.Add(configure);
        }

        public void AddReceiveEndpointDependency(IReceiveEndpointConfigurator dependency)
        {
            _dependencies.Add(dependency);
        }

        public void Configure(IServiceInstanceConfigurator instanceConfigurator, IRegistrationContext context)
        {
            AddReceiveEndpointDependency(instanceConfigurator.InstanceEndpointConfigurator);

            Settings.JobService.ConfigureSuperviseJobConsumer(instanceConfigurator.InstanceEndpointConfigurator);

            if (instanceConfigurator.BusConfigurator is IBusObserverConnector connector)
                connector.ConnectBusObserver(new JobServiceBusObserver(Settings.JobService));

            instanceConfigurator.ConnectEndpointConfigurationObserver(new JobServiceEndpointConfigurationObserver(Settings, ConfigureJobConsumerEndpoint));
        }

        void ConfigureJobConsumerEndpoint(IReceiveEndpointConfigurator configurator)
        {
            foreach (var dependency in _dependencies)
                configurator.AddDependency(dependency);
        }

        InstanceJobServiceSettings GetJobServiceSettings()
        {
            var options = new JobConsumerOptions();
            foreach (Action<JobConsumerOptions> configure in _configureActions)
                configure(options);

            return new InstanceJobServiceSettings(options);
        }
    }
}

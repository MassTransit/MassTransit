namespace MassTransit.RabbitMqTransport.Hosting
{
    using Configurators;
    using Context;
    using MassTransit.Hosting;


    public class RabbitMqHostBusFactory :
        IHostBusFactory
    {
        readonly ConfigurationHostSettings _hostSettings;

        public RabbitMqHostBusFactory(ISettingsProvider settingsProvider)
        {
            if (!settingsProvider.TryGetSettings("RabbitMQ", out RabbitMqSettings settings))
                throw new ConfigurationException("The RabbitMQ settings were not available");

            _hostSettings = new ConfigurationHostSettings
            {
                Host = settings.Host ?? "[::1]",
                Port = settings.Port ?? 5672,
                VirtualHost = string.IsNullOrWhiteSpace(settings.VirtualHost) ? "/" : settings.VirtualHost.Trim('/'),
                Username = settings.Username ?? "guest",
                Password = settings.Password ?? "guest",
                Heartbeat = settings.Heartbeat ?? 0,
            };

            if (!string.IsNullOrWhiteSpace(settings.ClusterMembers))
            {
                var configurator = new RabbitMqClusterConfigurator(_hostSettings);
                foreach (var nodeAddress in settings.ClusterMembers.Split(','))
                    configurator.Node(nodeAddress);

                _hostSettings.EndpointResolver = configurator.GetEndpointResolver();
            }
        }

        public IBusControl CreateBus(IBusServiceConfigurator busServiceConfigurator, string serviceName)
        {
            return RabbitMqBusFactory.Create(configurator =>
            {
                configurator.Host(_hostSettings);

                LogContext.Info?.Log("Configuring Host: {Host}", _hostSettings.ToDescription());

                var serviceConfigurator = new RabbitMqServiceConfigurator(configurator);

                busServiceConfigurator.Configure(serviceConfigurator);
            });
        }
    }
}

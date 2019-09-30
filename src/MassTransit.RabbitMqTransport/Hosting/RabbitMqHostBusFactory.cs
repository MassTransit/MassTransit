namespace MassTransit.RabbitMqTransport.Hosting
{
    using Configurators;
    using Context;
    using MassTransit.Hosting;


    public class RabbitMqHostBusFactory :
        IHostBusFactory
    {
        readonly RabbitMqHostSettings _hostSettings;

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
                ClusterMembers = settings.ClusterMembers?.Split(',')
            };
        }

        public IBusControl CreateBus(IBusServiceConfigurator busServiceConfigurator, string serviceName)
        {
            return RabbitMqBusFactory.Create(configurator =>
            {
                var host = configurator.Host(_hostSettings);

                LogContext.Info?.Log("Configuring Host: {Host}", _hostSettings.ToDescription());

                var serviceConfigurator = new RabbitMqServiceConfigurator(configurator);

                busServiceConfigurator.Configure(serviceConfigurator);
            });
        }
    }
}

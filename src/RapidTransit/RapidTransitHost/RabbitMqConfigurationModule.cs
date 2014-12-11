namespace RapidTransit
{
    using System.Configuration;
    using Autofac;
    using Configuration;


    public class RabbitMqConfigurationModule :
        Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.Register(GetRabbitMqSettings)
                   .As<RabbitMqSettings>()
                   .SingleInstance();

            builder.RegisterType<RabbitMqTransportConfigurator>()
                   .As<ITransportConfigurator>();
        }

        static RabbitMqSettings GetRabbitMqSettings(IComponentContext context)
        {
            RabbitMqSettings settings;
            if (context.Resolve<ISettingsProvider>().TryGetSettings("RabbitMQ", out settings))
                return settings;

            throw new ConfigurationErrorsException("Unable to resolve RabbitMqSettings from configuration");
        }
    }
}
namespace RapidTransit
{
    using Autofac;
    using RapidTransit.Configuration;


    public class ServiceConfigurationProviderModule :
        ConfigurationProviderModule
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<FileConfigurationProvider>()
                   .As<IConfigurationProvider>()
                   .SingleInstance();

            base.Load(builder);
        }
    }
}
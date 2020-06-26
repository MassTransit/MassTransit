namespace MassTransit.Conductor.Configurators
{
    using System;


    public class ServiceClientConfigurator :
        IServiceClientConfigurator
    {
        public ServiceClientConfigurator()
        {
            Options = new ServiceClientOptions();
        }

        public ServiceClientOptions Options { get; }

        T IOptionsSet.Options<T>(Action<T> configure)
        {
            return Options.Options(configure);
        }

        bool IOptionsSet.TryGetOptions<T>(out T options)
        {
            return Options.TryGetOptions(out options);
        }
    }
}

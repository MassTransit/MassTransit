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

        T IOptionsSet.Options<T>(T options, Action<T> configure)
        {
            return Options.Options(options, configure);
        }

        bool IOptionsSet.TryGetOptions<T>(out T options)
        {
            return Options.TryGetOptions(out options);
        }
    }
}

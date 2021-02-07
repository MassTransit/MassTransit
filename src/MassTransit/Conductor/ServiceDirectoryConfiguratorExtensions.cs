namespace MassTransit.Conductor
{
    using System;
    using Directory;


    public static class ServiceDirectoryConfiguratorExtensions
    {
        public static void AddMessageInitializer<TInput, TResult>(this IServiceDirectoryConfigurator directoryConfigurator,
            ResultValueProvider<TInput> provider,
            Action<IServiceRegistrationConfigurator<TInput>> configure = default)
            where TInput : class
            where TResult : class
        {
            directoryConfigurator.AddService<TInput, TResult>(x => x.Initializer(provider), configure);
        }

        public static void AddMessageFactory<TInput, TResult>(this IServiceDirectoryConfigurator directoryConfigurator,
            AsyncMessageFactory<TInput, TResult> factory,
            Action<IServiceRegistrationConfigurator<TInput>> configure = default)
            where TInput : class
            where TResult : class
        {
            directoryConfigurator.AddService<TInput, TResult>(x => x.Factory(factory), configure);
        }

        public static void AddMessageFactory<TInput, TResult>(this IServiceDirectoryConfigurator directoryConfigurator, MessageFactory<TInput, TResult> factory,
            Action<IServiceRegistrationConfigurator<TInput>> configure = default)
            where TInput : class
            where TResult : class
        {
            directoryConfigurator.AddService<TInput, TResult>(x => x.Factory(factory), configure);
        }
    }
}

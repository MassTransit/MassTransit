namespace MassTransit.Conductor.Directory
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using GreenPipes;
    using GreenPipes.Internals.Extensions;
    using Internals.Extensions;
    using Internals.GraphValidation;
    using Observers;


    public class ServiceDirectoryConfigurator :
        IServiceDirectoryConfigurator,
        IDisposable
    {
        readonly List<ConnectHandle> _configurationHandles;
        readonly Dictionary<Type, IServiceRegistration> _registrations;

        public ServiceDirectoryConfigurator()
        {
            _registrations = new Dictionary<Type, IServiceRegistration>();
            _configurationHandles = new List<ConnectHandle>();
        }

        public void Dispose()
        {
            foreach (var configurationHandle in _configurationHandles)
                configurationHandle.Dispose();

            _configurationHandles.Clear();
        }

        public void AddService<TInput, TResult>(Func<IServiceProviderSelector<TInput, TResult>, IServiceProviderDefinition<TInput, TResult>> providerSelector,
            Action<IServiceRegistrationConfigurator<TInput>> configure = default)
            where TResult : class
            where TInput : class
        {
            var configurator = new ServiceProviderSelector<TInput, TResult>();

            IServiceProviderDefinition<TInput, TResult> definition = providerSelector(configurator);
            if (definition == null)
                throw new ConfigurationException("The definition was null");

            GetServiceRegistration<TInput>();

            IServiceRegistration<TResult> registration = GetServiceRegistration<TResult>();

            var registrationConfigurator = new ServiceRegistrationConfigurator<TInput, TResult>(registration, definition);

            definition.Configure(registrationConfigurator);

            configure?.Invoke(registrationConfigurator);

            registrationConfigurator.ConfigureProvider();
        }

        IServiceRegistration<TResult> GetServiceRegistration<TResult>()
            where TResult : class
        {
            var registration = _registrations.GetOrAdd(typeof(TResult), _ => new ServiceRegistration<TResult>());

            return registration as IServiceRegistration<TResult> ?? throw new ArgumentException(
                $"Registration type mismatch, expected {TypeCache<TResult>.ShortName}, was {TypeCache.GetShortName(registration.GetType())}",
                nameof(registration));
        }

        public IServiceDirectory CreateServiceDirectory()
        {
            ValidateRegistration();

            return new ServiceDirectory(_registrations);
        }

        public void Connect(IBusFactoryConfigurator configurator)
        {
            var configurationObserver = new ServiceDirectoryConfigurationObserver(configurator, this);

            _configurationHandles.Add(configurator.ConnectEndpointConfigurationObserver(configurationObserver));
        }

        public void AddConfigurationHandle(ConnectHandle handle)
        {
            _configurationHandles.Add(handle);
        }

        void ValidateRegistration()
        {
            var graph = new DependencyGraph<Type>(_registrations.Count);

            foreach (var registration in _registrations.Values)
            {
                graph.Add(registration.ServiceType);

                foreach (var provider in registration.Providers)
                    graph.Add(provider.InputType, registration.ServiceType);
            }

            graph.EnsureGraphIsAcyclic();
        }

        public void OnConfigureInput<TMessage>(IReceiveEndpointConfigurator configurator)
            where TMessage : class
        {
            foreach (var providerRegistration in _registrations.Values.SelectMany(x => x.Providers))
            {
                if (providerRegistration.InputType == typeof(TMessage))
                    providerRegistration.OnConfigureReceiveEndpoint(configurator);
            }
        }
    }
}

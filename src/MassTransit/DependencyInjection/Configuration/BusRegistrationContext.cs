#nullable enable
namespace MassTransit.Configuration
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using DependencyInjection.Registration;
    using Internals;


    public class BusRegistrationContext :
        RegistrationContext,
        IBusRegistrationContext
    {
        IConfigureReceiveEndpoint? _configureReceiveEndpoints;

        public BusRegistrationContext(IServiceProvider provider, IContainerSelector selector, ISetScopedConsumeContext setScopedConsumeContext)
            : base(provider, selector, setScopedConsumeContext)
        {
        }

        public IEndpointNameFormatter EndpointNameFormatter => Selector.GetEndpointNameFormatter(this);

        public void ConfigureEndpoints<T>(IReceiveConfigurator<T> configurator, IEndpointNameFormatter? endpointNameFormatter = null)
            where T : IReceiveEndpointConfigurator
        {
            ConfigureEndpoints(configurator, endpointNameFormatter, NoFilter);
        }

        public void ConfigureEndpoints<T>(IReceiveConfigurator<T> configurator, IEndpointNameFormatter? endpointNameFormatter,
            Action<IRegistrationFilterConfigurator>? configureFilter)
            where T : IReceiveEndpointConfigurator
        {
            endpointNameFormatter ??= EndpointNameFormatter;

            var builder = new RegistrationFilterConfigurator();
            configureFilter?.Invoke(builder);

            var registrationFilter = builder.Filter;

            List<IGrouping<string, IConsumerDefinition>> consumersByEndpoint = Selector.GetRegistrations<IConsumerRegistration>(this)
                .Where(x => x.IncludeInConfigureEndpoints && !WasConfigured(x.Type) && registrationFilter.Matches(x))
                .Select(x => x.GetDefinition(this))
                .GroupBy(x => x.GetEndpointName(endpointNameFormatter))
                .ToList();

            List<IGrouping<string, ISagaDefinition>> sagasByEndpoint = Selector.GetRegistrations<ISagaRegistration>(this)
                .Where(x => x.IncludeInConfigureEndpoints && !WasConfigured(x.Type) && registrationFilter.Matches(x))
                .Select(x => x.GetDefinition(this))
                .GroupBy(x => x.GetEndpointName(endpointNameFormatter))
                .ToList();

            List<IActivityDefinition> activities = Selector.GetRegistrations<IActivityRegistration>(this)
                .Where(x => x.IncludeInConfigureEndpoints && !WasConfigured(x.Type) && registrationFilter.Matches(x))
                .Select(x => x.GetDefinition(this))
                .ToList();

            List<IGrouping<string, IActivityDefinition>> activitiesByExecuteEndpoint = activities
                .GroupBy(x => x.GetExecuteEndpointName(endpointNameFormatter))
                .ToList();

            List<IGrouping<string, IActivityDefinition>> activitiesByCompensateEndpoint = activities
                .GroupBy(x => x.GetCompensateEndpointName(endpointNameFormatter))
                .ToList();

            List<IGrouping<string, IExecuteActivityDefinition>> executeActivitiesByEndpoint = Selector.GetRegistrations<IExecuteActivityRegistration>(this)
                .Where(x => x.IncludeInConfigureEndpoints && !WasConfigured(x.Type) && registrationFilter.Matches(x))
                .Select(x => x.GetDefinition(this))
                .GroupBy(x => x.GetExecuteEndpointName(endpointNameFormatter))
                .ToList();

            List<IGrouping<string, IFutureDefinition>> futuresByEndpoint = Selector.GetRegistrations<IFutureRegistration>(this)
                .Where(x => x.IncludeInConfigureEndpoints && !WasConfigured(x.Type) && registrationFilter.Matches(x))
                .Select(x => x.GetDefinition(this))
                .GroupBy(x => x.GetEndpointName(endpointNameFormatter))
                .ToList();

            var endpointsWithName = Selector.GetRegistrations<IEndpointRegistration>(this)
                .Where(x => x.IncludeInConfigureEndpoints && !WasConfigured(x.Type) && registrationFilter.Matches(x))
                .Select(x => x.GetDefinition(this))
                .Select(x => new
                {
                    Name = x.GetEndpointName(endpointNameFormatter),
                    Definition = x
                })
                .GroupBy(x => x.Name, (name, values) => new
                {
                    Name = name,
                    Definition = values.Select(x => x.Definition).Combine(this)
                })
                .ToList();

            IEndpointDefinition? GetEndpointDefinitionByName(string name)
            {
                return endpointsWithName.SingleOrDefault(x => x.Name == name)?.Definition;
            }

            IEnumerable<string> endpointNames = consumersByEndpoint.Select(x => x.Key)
                .Union(sagasByEndpoint.Select(x => x.Key))
                .Union(activitiesByExecuteEndpoint.Select(x => x.Key))
                .Union(executeActivitiesByEndpoint.Select(x => x.Key))
                .Union(futuresByEndpoint.Select(x => x.Key))
                .Union(endpointsWithName.Select(x => x.Name))
                .Except(activitiesByCompensateEndpoint.Select(x => x.Key));

            IList<Endpoint> endpoints = (
                from e in endpointNames
                join c in consumersByEndpoint on e equals c.Key into cs
                from c in cs.DefaultIfEmpty()
                join s in sagasByEndpoint on e equals s.Key into ss
                from s in ss.DefaultIfEmpty()
                join a in activitiesByExecuteEndpoint on e equals a.Key into aes
                from a in aes.DefaultIfEmpty()
                join ea in executeActivitiesByEndpoint on e equals ea.Key into eas
                from ea in eas.DefaultIfEmpty()
                join f in futuresByEndpoint on e equals f.Key into fs
                from f in fs.DefaultIfEmpty()
                join ep in endpointsWithName on e equals ep.Name into eps
                from ep in eps.Select(x => x.Definition)
                    .DefaultIfEmpty(c?.Select(x => (IEndpointDefinition)new DelegateEndpointDefinition(e, x, x.EndpointDefinition)).Combine(this)
                        ?? s?.Select(x => (IEndpointDefinition)new DelegateEndpointDefinition(e, x, x.EndpointDefinition)).Combine(this)
                        ?? a?.Select(x => (IEndpointDefinition)new DelegateEndpointDefinition(e, x, x.ExecuteEndpointDefinition)).Combine(this)
                        ?? ea?.Select(x => (IEndpointDefinition)new DelegateEndpointDefinition(e, x, x.ExecuteEndpointDefinition)).Combine(this)
                        ?? f?.Select(x => (IEndpointDefinition)new DelegateEndpointDefinition(e, x, x.EndpointDefinition)).Combine(this)
                        ?? new NamedEndpointDefinition(e))
                select new Endpoint(ep, c, s, a, ea, f)).ToList();

            var needsServiceInstance = !(configurator is IServiceInstanceConfigurator<T>) && endpoints.Any(endpoint => endpoint.HasJobConsumers);
            if (needsServiceInstance)
            {
                var registration = Selector.GetRegistrations<IJobServiceRegistration>(this).SingleOrDefault();
                registration ??= new JobServiceRegistration();

                var endpointDefinition = new RegistrationContextEndpointDefinition(registration.EndpointDefinition, this);

                configurator.ReceiveEndpoint(endpointDefinition, endpointNameFormatter, endpointConfigurator =>
                {
                    var options = new ServiceInstanceOptions().SetEndpointNameFormatter(endpointNameFormatter);

                    var instanceConfigurator = new ServiceInstanceConfigurator<T>(configurator, options, endpointConfigurator);

                    registration.Configure(instanceConfigurator, this);

                    ConfigureTheEndpoints(endpoints, endpointNameFormatter, GetEndpointDefinitionByName, configurator, instanceConfigurator);
                });
            }
            else
                ConfigureTheEndpoints(endpoints, endpointNameFormatter, GetEndpointDefinitionByName, configurator);
        }

        public IConfigureReceiveEndpoint GetConfigureReceiveEndpoints()
        {
            if (_configureReceiveEndpoints != null)
                return _configureReceiveEndpoints;

            _configureReceiveEndpoints = Selector.GetConfigureReceiveEndpoints(this);

            return _configureReceiveEndpoints;
        }

        void ConfigureTheEndpoints<T>(IEnumerable<Endpoint> endpoints, IEndpointNameFormatter endpointNameFormatter,
            Func<string, IEndpointDefinition?> getEndpointDefinitionByName,
            IReceiveConfigurator<T> configurator, IReceiveConfigurator<T>? instanceConfigurator = null)
            where T : IReceiveEndpointConfigurator
        {
            var configureReceiveEndpoint = GetConfigureReceiveEndpoints();

            foreach (var endpoint in endpoints)
            {
                IReceiveConfigurator<T> useConfigurator = instanceConfigurator != null && endpoint.HasJobConsumers
                    ? instanceConfigurator
                    : configurator;

                var endpointDefinition = new RegistrationContextEndpointDefinition(endpoint.Definition, this);

                useConfigurator.ReceiveEndpoint(endpointDefinition, endpointNameFormatter, cfg =>
                {
                    configureReceiveEndpoint.Configure(endpointDefinition.GetEndpointName(endpointNameFormatter), cfg);

                    foreach (var consumer in endpoint.Consumers)
                        ConfigureConsumer(consumer.ConsumerType, cfg);

                    foreach (var saga in endpoint.Sagas)
                        ConfigureSaga(saga.SagaType, cfg);

                    foreach (var activity in endpoint.Activities)
                    {
                        var compensateEndpointName = activity.GetCompensateEndpointName(endpointNameFormatter);

                        var compensateDefinition = activity.CompensateEndpointDefinition ?? getEndpointDefinitionByName(compensateEndpointName);
                        if (compensateDefinition != null)
                        {
                            compensateDefinition = new RegistrationContextEndpointDefinition(compensateDefinition, this);
                            configurator.ReceiveEndpoint(compensateDefinition, endpointNameFormatter, compensateEndpointConfigurator =>
                            {
                                configureReceiveEndpoint.Configure(compensateDefinition.GetEndpointName(endpointNameFormatter),
                                    compensateEndpointConfigurator);

                                ConfigureActivity(activity.ActivityType, cfg, compensateEndpointConfigurator);
                            });
                        }
                        else
                        {
                            configurator.ReceiveEndpoint(compensateEndpointName, compensateEndpointConfigurator =>
                            {
                                configureReceiveEndpoint.Configure(compensateEndpointName, compensateEndpointConfigurator);

                                ConfigureActivity(activity.ActivityType, cfg, compensateEndpointConfigurator);
                            });
                        }
                    }

                    foreach (var activity in endpoint.ExecuteActivities)
                        ConfigureExecuteActivity(activity.ActivityType, cfg);

                    foreach (var future in endpoint.Futures)
                        ConfigureFuture(future.FutureType, cfg);
                });
            }
        }

        static void NoFilter(IRegistrationFilterConfigurator configurator)
        {
        }


        class Endpoint
        {
            public Endpoint(IEndpointDefinition definition, IEnumerable<IConsumerDefinition>? consumers, IEnumerable<ISagaDefinition>? sagas,
                IEnumerable<IActivityDefinition>? activities, IEnumerable<IExecuteActivityDefinition>? executeActivities,
                IEnumerable<IFutureDefinition>? futures)
            {
                Definition = definition;
                Consumers = consumers?.ToList() ?? new List<IConsumerDefinition>();
                Sagas = sagas?.ToList() ?? new List<ISagaDefinition>();
                Activities = activities?.ToList() ?? new List<IActivityDefinition>();
                ExecuteActivities = executeActivities?.ToList() ?? new List<IExecuteActivityDefinition>();
                Futures = futures?.ToList() ?? new List<IFutureDefinition>();
            }

            public IEndpointDefinition Definition { get; }
            public List<IConsumerDefinition> Consumers { get; }
            public List<ISagaDefinition> Sagas { get; }
            public List<IActivityDefinition> Activities { get; }
            public List<IExecuteActivityDefinition> ExecuteActivities { get; }
            public List<IFutureDefinition> Futures { get; }

            public bool HasJobConsumers => Consumers.Any(c => c.ConsumerType.ClosesType(typeof(IJobConsumer<>)));
        }
    }
}

namespace MassTransit.Registration
{
    using System.Collections.Generic;
    using System.Linq;
    using Definition;
    using Monitoring.Health;


    public class BusRegistrationContext :
        Registration,
        IBusRegistrationContext
    {
        readonly BusHealth _busHealth;
        readonly IRegistrationCache<IEndpointRegistration> _endpoints;

        public BusRegistrationContext(IConfigurationServiceProvider provider, BusHealth busHealth, IRegistrationCache<IEndpointRegistration> endpoints,
            IRegistrationCache<IConsumerRegistration> consumers, IRegistrationCache<ISagaRegistration> sagas,
            IRegistrationCache<IExecuteActivityRegistration> executeActivities, IRegistrationCache<IActivityRegistration> activities)
            : base(provider, consumers, sagas, executeActivities, activities)
        {
            _busHealth = busHealth;
            _endpoints = endpoints;
        }

        public void UseHealthCheck(IBusFactoryConfigurator configurator)
        {
            configurator.ConnectBusObserver(_busHealth);
            configurator.ConnectEndpointConfigurationObserver(_busHealth);
        }

        public void ConfigureEndpoints<T>(IReceiveConfigurator<T> configurator, IEndpointNameFormatter endpointNameFormatter = null)
            where T : IReceiveEndpointConfigurator
        {
            endpointNameFormatter ??= GetService<IEndpointNameFormatter>() ?? DefaultEndpointNameFormatter.Instance;

            IEnumerable<IGrouping<string, IConsumerDefinition>> consumersByEndpoint = Consumers.Values
                .Select(x => x.GetDefinition(this))
                .GroupBy(x => x.GetEndpointName(endpointNameFormatter));

            IEnumerable<IGrouping<string, ISagaDefinition>> sagasByEndpoint = Sagas.Values
                .Select(x => x.GetDefinition(this))
                .GroupBy(x => x.GetEndpointName(endpointNameFormatter));

            IActivityDefinition[] activities = Activities.Values
                .Select(x => x.GetDefinition(this))
                .ToArray();

            IEnumerable<IGrouping<string, IActivityDefinition>> activitiesByExecuteEndpoint = activities
                .GroupBy(x => x.GetExecuteEndpointName(endpointNameFormatter));

            IEnumerable<IGrouping<string, IActivityDefinition>> activitiesByCompensateEndpoint = activities
                .GroupBy(x => x.GetCompensateEndpointName(endpointNameFormatter));

            IEnumerable<IGrouping<string, IExecuteActivityDefinition>> executeActivitiesByEndpoint = ExecuteActivities.Values
                .Select(x => x.GetDefinition(this))
                .GroupBy(x => x.GetExecuteEndpointName(endpointNameFormatter));

            var endpointsWithName = _endpoints.Values
                .Select(x => x.GetDefinition(this))
                .Select(x => new
                {
                    Name = x.GetEndpointName(endpointNameFormatter),
                    Definition = x
                })
                .GroupBy(x => x.Name, (name, values) => new
                {
                    Name = name,
                    Definition = values.Select(x => x.Definition).Combine()
                });

            IEnumerable<string> endpointNames = consumersByEndpoint.Select(x => x.Key)
                .Union(sagasByEndpoint.Select(x => x.Key))
                .Union(activitiesByExecuteEndpoint.Select(x => x.Key))
                .Union(executeActivitiesByEndpoint.Select(x => x.Key))
                .Union(endpointsWithName.Select(x => x.Name))
                .Except(activitiesByCompensateEndpoint.Select(x => x.Key));

            var endpoints =
                from e in endpointNames
                join c in consumersByEndpoint on e equals c.Key into cs
                from c in cs.DefaultIfEmpty()
                join s in sagasByEndpoint on e equals s.Key into ss
                from s in ss.DefaultIfEmpty()
                join a in activitiesByExecuteEndpoint on e equals a.Key into aes
                from a in aes.DefaultIfEmpty()
                join ea in executeActivitiesByEndpoint on e equals ea.Key into eas
                from ea in eas.DefaultIfEmpty()
                join ep in endpointsWithName on e equals ep.Name into eps
                from ep in eps.Select(x => x.Definition)
                    .DefaultIfEmpty(c?.Select(x => (IEndpointDefinition)new DelegateEndpointDefinition(e, x, x.EndpointDefinition)).Combine()
                        ?? s?.Select(x => (IEndpointDefinition)new DelegateEndpointDefinition(e, x, x.EndpointDefinition)).Combine()
                        ?? a?.Select(x => (IEndpointDefinition)new DelegateEndpointDefinition(e, x, x.ExecuteEndpointDefinition)).Combine()
                        ?? ea?.Select(x => (IEndpointDefinition)new DelegateEndpointDefinition(e, x, x.ExecuteEndpointDefinition)).Combine()
                        ?? new NamedEndpointDefinition(e))
                select new
                {
                    Name = e,
                    Definition = ep,
                    Consumers = c,
                    Sagas = s,
                    Activities = a,
                    ExecuteActivities = ea
                };

            foreach (var endpoint in endpoints)
            {
                configurator.ReceiveEndpoint(endpoint.Definition, endpointNameFormatter, cfg =>
                {
                    if (endpoint.Consumers != null)
                    {
                        foreach (var consumer in endpoint.Consumers)
                            ConfigureConsumer(consumer.ConsumerType, cfg);
                    }

                    if (endpoint.Sagas != null)
                    {
                        foreach (var saga in endpoint.Sagas)
                            ConfigureSaga(saga.SagaType, cfg);
                    }

                    if (endpoint.Activities != null)
                    {
                        foreach (var activity in endpoint.Activities)
                        {
                            var compensateEndpointName = activity.GetCompensateEndpointName(endpointNameFormatter);

                            var compensateDefinition = activity.CompensateEndpointDefinition ??
                                endpointsWithName.SingleOrDefault(x => x.Name == compensateEndpointName)?.Definition;

                            if (compensateDefinition != null)
                            {
                                configurator.ReceiveEndpoint(compensateDefinition, endpointNameFormatter, compensateEndpointConfigurator =>
                                {
                                    ConfigureActivity(activity.ActivityType, cfg, compensateEndpointConfigurator);
                                });
                            }
                            else
                            {
                                configurator.ReceiveEndpoint(compensateEndpointName, compensateEndpointConfigurator =>
                                {
                                    ConfigureActivity(activity.ActivityType, cfg, compensateEndpointConfigurator);
                                });
                            }
                        }
                    }

                    if (endpoint.ExecuteActivities != null)
                    {
                        foreach (var activity in endpoint.ExecuteActivities)
                            ConfigureExecuteActivity(activity.ActivityType, cfg);
                    }
                });
            }
        }
    }
}

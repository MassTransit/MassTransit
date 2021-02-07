namespace MassTransit.Conductor.Orchestration
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using GreenPipes.Internals.Extensions;
    using Internals.GraphValidation;


    public class OrchestrationPlanner<TResult> :
        IOrchestrationPlanner<TResult>
        where TResult : class
    {
        readonly DependencyGraph<IServiceRegistration> _inputGraph;
        readonly Dictionary<Type, IServiceRegistration> _registrations;

        public OrchestrationPlanner(IServiceRegistration[] serviceRegistrations)
        {
            if (serviceRegistrations == null || serviceRegistrations.Length == 0)
                throw new ArgumentException("need registrations", nameof(serviceRegistrations));

            _registrations = serviceRegistrations.ToDictionary(x => x.ServiceType);
            _inputGraph = BuildInputGraph(serviceRegistrations);
        }

        public IOrchestrationPlan<TInput, TResult> BuildExecutionPlan<TInput>()
            where TInput : class
        {
            var inputType = typeof(TInput);

            if (!_registrations.TryGetValue(inputType, out var inputRegistration))
                throw new ConfigurationException($"Input type not registered: {TypeCache.GetShortName(inputType)}");

            List<IServiceRegistration> registrations = _inputGraph.GetItemsInOrder(inputRegistration).ToList();

            var currentRegistration = registrations.First();
            if (currentRegistration.ServiceType != typeof(TResult))
                throw new ConfigurationException($"Response type not at the front: {TypeCache<TResult>.ShortName}");

            object previousOrchestrationStep = new ResultOrchestrationPlan<TResult>();

            foreach (var registration in registrations.Skip(1))
            {
                var nextType = registration.ServiceType;

                var providerRegistration = currentRegistration.Providers.Single(x => x.InputType == nextType);

                IOrchestrationPlanStep<TResult> orchestrationPlanStep = providerRegistration.CreateResolutionStep<TResult>();

                var stepType = typeof(NextOrchestrationPlan<,,>).MakeGenericType(orchestrationPlanStep.RequestType, orchestrationPlanStep.ServiceType,
                    typeof(TResult));
                previousOrchestrationStep = Activator.CreateInstance(stepType, orchestrationPlanStep, previousOrchestrationStep);

                if (nextType == inputType)
                    break;

                currentRegistration = registration;
            }

            return (IOrchestrationPlan<TInput, TResult>)previousOrchestrationStep;
        }

        DependencyGraph<IServiceRegistration> BuildInputGraph(IServiceRegistration[] registrations)
        {
            var graph = new DependencyGraph<IServiceRegistration>(registrations.Length + registrations.SelectMany(x => x.Providers).Count());

            foreach (var registration in registrations)
            {
                graph.Add(registration);

                foreach (var provider in registration.Providers)
                {
                    if (_registrations.TryGetValue(provider.InputType, out var inputRegistration))
                        graph.Add(inputRegistration, registration);
                }
            }

            graph.EnsureGraphIsAcyclic();

            return graph;
        }
    }
}

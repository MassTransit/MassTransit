namespace MassTransit.Transports
{
    using Automatonymous;
    using Automatonymous.Binders;
    using Context;
    using Monitoring.Health;


    public class ReceiveEndpointStateMachine :
        AutomatonymousStateMachine<ReceiveEndpoint>
    {
        // ReSharper disable UnassignedGetOnlyAutoProperty
        // ReSharper disable MemberCanBePrivate.Global
        public ReceiveEndpointStateMachine()
        {
            During(Initial, Started, Completed, Faulted,
                When(ReceiveEndpointReady)
                    .Then(x =>
                    {
                        x.Instance.Message = x.Data.IsStarted ? "ready" : "ready (not started)";
                        x.Instance.InputAddress = x.Data.InputAddress;
                    })
                    .SetHealthy()
                    .Then(x => LogContext.Debug?.Log("Endpoint Ready: {InputAddress}", x.Instance.InputAddress))
                    .TransitionTo(Ready));

            During(Ready, Ignore(ReceiveEndpointReady));


            During(Initial, Started, Ready, Completed,
                When(ReceiveEndpointFaulted)
                    .Then(x =>
                    {
                        x.Instance.Message = $"faulted ({x.Data.Exception.Message})";
                        x.Instance.InputAddress = x.Data.InputAddress;
                    })
                    .SetUnHealthy()
                    .Then(x => LogContext.Debug?.Log("Endpoint Faulted: {InputAddress}", x.Instance.InputAddress))
                    .TransitionTo(Faulted)
            );

            During(Faulted, Ignore(ReceiveEndpointFaulted));


            During(Initial, Started, Ready, Faulted,
                When(ReceiveEndpointCompleted)
                    .Then(x => x.Instance.Message = $"stopped (delivered {x.Data.DeliveryCount} messages)")
                    .SetDegraded()
                    .Then(x => LogContext.Debug?.Log("Endpoint Completed: {InputAddress}", x.Instance.InputAddress))
                    .TransitionTo(Completed)
            );

            During(Completed, Ignore(ReceiveEndpointCompleted));


            During(Initial, Completed, Faulted,
                When(ReceiveEndpointStarted)
                    .Then(x =>
                    {
                        x.Instance.Message = "starting";
                    })
                    .TransitionTo(Started)
            );

            During(Ready,
                Ignore(ReceiveEndpointStarted));

            During(Started, Ignore(ReceiveEndpointStarted));

            DuringAny(
                When(ReceiveEndpointStopping)
                    .Then(x => x.Instance.Message = "stopping")
                    .Then(x => LogContext.Debug?.Log("Endpoint Stopping: {InputAddress}", x.Instance.InputAddress))
            );
        }

        public State Started { get; }
        public State Ready { get; }
        public State Completed { get; }
        public State Faulted { get; }

        public Event ReceiveEndpointStarted { get; }

        public Event<ReceiveEndpointReady> ReceiveEndpointReady { get; }
        public Event<ReceiveEndpointStopping> ReceiveEndpointStopping { get; }
        public Event<ReceiveEndpointCompleted> ReceiveEndpointCompleted { get; }
        public Event<ReceiveEndpointFaulted> ReceiveEndpointFaulted { get; }

        public bool IsStarted(ReceiveEndpoint instance)
        {
            return Started.Equals(instance.CurrentState) || Ready.Equals(instance.CurrentState) || Faulted.Equals(instance.CurrentState);
        }
    }


    public static class ReceiveEndpointStateMachineExtensions
    {
        public static EventActivityBinder<ReceiveEndpoint, T> SetHealthy<T>(this EventActivityBinder<ReceiveEndpoint, T> binder)
            where T : class
        {
            return binder.Then(context => context.Instance.HealthResult = EndpointHealthResult.Healthy(context.Instance, context.Instance.Message));
        }

        public static EventActivityBinder<ReceiveEndpoint, T> SetDegraded<T>(this EventActivityBinder<ReceiveEndpoint, T> binder)
            where T : class
        {
            return binder.Then(context => context.Instance.HealthResult = EndpointHealthResult.Degraded(context.Instance, context.Instance.Message));
        }

        public static EventActivityBinder<ReceiveEndpoint, ReceiveEndpointFaulted> SetUnHealthy(this EventActivityBinder<ReceiveEndpoint,
            ReceiveEndpointFaulted> binder)
        {
            return binder.Then(context =>
                context.Instance.HealthResult = EndpointHealthResult.Unhealthy(context.Instance, context.Instance.Message, context.Data.Exception));
        }
    }
}

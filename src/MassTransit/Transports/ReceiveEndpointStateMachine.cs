namespace MassTransit.Transports
{
    using Automatonymous;
    using Automatonymous.Binders;
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
                    .TransitionTo(Faulted)
            );

            During(Faulted, Ignore(ReceiveEndpointFaulted));


            During(Initial, Started, Ready, Faulted,
                When(ReceiveEndpointCompleted)
                    .Then(x => x.Instance.Message = $"stopped (delivered {x.Data.DeliveryCount} messages)")
                    .SetDegraded()
                    .TransitionTo(Completed)
            );

            During(Completed, Ignore(ReceiveEndpointCompleted));


            During(Initial, Completed, Faulted,
                When(ReceiveEndpointStarted)
                    .Then(x =>
                    {
                        x.Instance.EndpointHandle = x.Data;
                        x.Instance.Message = "starting";
                    })
                    .TransitionTo(Started)
            );

            During(Ready,
                When(ReceiveEndpointStarted)
                    .Then(x => x.Instance.EndpointHandle = x.Data));

            During(Started, Ignore(ReceiveEndpointStarted));

            DuringAny(
                When(ReceiveEndpointStopping)
                    .Then(x => x.Instance.Message = "stopping"));
        }

        public State Started { get; }
        public State Ready { get; }
        public State Completed { get; }
        public State Faulted { get; }

        public Event<HostReceiveEndpointHandle> ReceiveEndpointStarted { get; }

        public Event<ReceiveEndpointReady> ReceiveEndpointReady { get; }
        public Event<ReceiveEndpointStopping> ReceiveEndpointStopping { get; }
        public Event<ReceiveEndpointCompleted> ReceiveEndpointCompleted { get; }
        public Event<ReceiveEndpointFaulted> ReceiveEndpointFaulted { get; }

        public bool IsStarted(ReceiveEndpoint instance)
        {
            return Started.Equals(instance.CurrentState) || Ready.Equals(instance.CurrentState);
        }
    }


    public static class ReceiveEndpointStateMachineExtensions
    {
        public static EventActivityBinder<ReceiveEndpoint, T> SetHealthy<T>(this EventActivityBinder<ReceiveEndpoint, T> binder)
            where T : class
        {
            return binder.Then(context => context.Instance.HealthResult = HealthResult.Healthy(context.Instance.Message));
        }

        public static EventActivityBinder<ReceiveEndpoint, T> SetDegraded<T>(this EventActivityBinder<ReceiveEndpoint, T> binder)
            where T : class
        {
            return binder.Then(context => context.Instance.HealthResult = HealthResult.Degraded(context.Instance.Message));
        }

        public static EventActivityBinder<ReceiveEndpoint, ReceiveEndpointFaulted> SetUnHealthy(this EventActivityBinder<ReceiveEndpoint,
            ReceiveEndpointFaulted> binder)
        {
            return binder.Then(context =>
                context.Instance.HealthResult = HealthResult.Unhealthy(context.Instance.Message, context.Data.Exception));
        }
    }
}

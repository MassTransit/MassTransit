namespace MassTransit.Turnout.Components.StateMachines
{
    using Automatonymous;
    using Automatonymous.Binders;
    using GreenPipes;
    using MassTransit.Contracts.Turnout;


    public sealed class TurnoutJobAttemptStateMachine :
        MassTransitStateMachine<TurnoutJobAttemptState>
    {
        public TurnoutJobAttemptStateMachine(TurnoutOptions options)
        {
            Event(() => StartJobAttempt, x => x.CorrelateById(context => context.Message.AttemptId));

            Event(() => StartJobFaulted, x => x.CorrelateById(context => context.Message.Message.AttemptId));

            Event(() => AttemptStarted, x => x.CorrelateById(context => context.Message.AttemptId));
            Event(() => AttemptCompleted, x => x.CorrelateById(context => context.Message.AttemptId));
            Event(() => AttemptFaulted, x => x.CorrelateById(context => context.Message.AttemptId));
            Event(() => AttemptCanceled, x => x.CorrelateById(context => context.Message.AttemptId));

            Schedule(() => StatusCheckRequested, instance => instance.StatusCheckTokenId, x =>
            {
                x.Delay = options.JobStatusCheckInterval;
                x.Received = r => r.CorrelateById(context => context.Message.AttemptId);
            });

            InstanceState(x => x.CurrentState, Starting, Running, Faulted, Waiting);

            During(Initial, Starting,
                When(StartJobAttempt)
                    .Then(context =>
                    {
                        context.Instance.JobId = context.Data.JobId;
                        context.Instance.RetryAttempt = context.Data.RetryAttempt;
                        context.Instance.ServiceAddress ??= context.Data.ServiceAddress;
                    })
                    .SendStartJob()
                    .RespondAsync(context => context.Init<JobAttemptCreated>(context.Data))
                    .TransitionTo(Starting));

            During(Starting,
                When(StartJobFaulted)
                    .Then(context =>
                    {
                        context.Instance.Faulted = context.Data.Timestamp;
                        context.Instance.InstanceAddress ??= context.GetPayload<ConsumeContext>().SourceAddress;
                    })
                    .PublishJobAttemptFaulted()
                    .TransitionTo(Faulted));

            During(Initial, Starting, Running,
                When(AttemptStarted)
                    .Then(context =>
                    {
                        context.Instance.JobId = context.Data.JobId;
                        context.Instance.Started = context.Data.Timestamp;
                        context.Instance.RetryAttempt = context.Data.RetryAttempt;
                        context.Instance.InstanceAddress ??= context.Data.InstanceAddress;
                    })
                    .Schedule(StatusCheckRequested, x => x.Init<JobStatusCheckRequested>(new {AttemptId = x.Instance.CorrelationId}))
                    .TransitionTo(Running));

            During(Running,
                When(AttemptCompleted)
                    .Unschedule(StatusCheckRequested)
                    .Finalize(),
                When(AttemptCanceled)
                    .Unschedule(StatusCheckRequested)
                    .Finalize(),
                When(AttemptFaulted)
                    .Then(context =>
                    {
                        context.Instance.Faulted = context.Data.Timestamp;
                        context.Instance.InstanceAddress ??= context.GetPayload<ConsumeContext>().SourceAddress;
                    })
                    .Unschedule(StatusCheckRequested)
                    .TransitionTo(Faulted));

            During(Running,
                When(StatusCheckRequested.Received));

            During(Faulted,
                Ignore(StatusCheckRequested.Received));
        }

        public State Waiting { get; private set; }
        public State Starting { get; private set; }
        public State Running { get; private set; }
        public State Faulted { get; private set; }

        public Event<StartJobAttempt> StartJobAttempt { get; private set; }

        public Event<Fault<StartJob>> StartJobFaulted { get; private set; }

        public Event<JobAttemptStarted> AttemptStarted { get; private set; }
        public Event<JobAttemptFaulted> AttemptFaulted { get; private set; }
        public Event<JobAttemptCompleted> AttemptCompleted { get; private set; }
        public Event<JobAttemptCanceled> AttemptCanceled { get; private set; }

        public Schedule<TurnoutJobAttemptState, JobStatusCheckRequested> StatusCheckRequested { get; private set; }
    }


    static class TurnoutJobAttemptStateMachineBehaviorExtensions
    {
        public static EventActivityBinder<TurnoutJobAttemptState, StartJobAttempt> SendStartJob(
            this EventActivityBinder<TurnoutJobAttemptState, StartJobAttempt> binder)
        {
            return binder.SendAsync(context => context.Instance.ServiceAddress, context => context.Init<StartJob>(new
            {
                context.Data.JobId,
                context.Data.AttemptId,
                context.Data.RetryAttempt,
                context.Data.Job
            }));
        }

        public static EventActivityBinder<TurnoutJobAttemptState, Fault<StartJob>> PublishJobAttemptFaulted(
            this EventActivityBinder<TurnoutJobAttemptState, Fault<StartJob>> binder)
        {
            return binder.PublishAsync(context => context.Init<JobAttemptFaulted>(new
            {
                context.Data.Message.JobId,
                context.Data.Message.AttemptId,
                context.Data.Message.RetryAttempt,
                context.Data.Message.Job,
                context.Data.Timestamp,
                context.Data.Exceptions
            }));
        }
    }
}

namespace MassTransit.Turnout.Components.StateMachines
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using Automatonymous;
    using Automatonymous.Binders;
    using Automatonymous.Events;
    using Courier;
    using GreenPipes;
    using MassTransit.Contracts.Turnout;


    public sealed class TurnoutJobStateMachine :
        MassTransitStateMachine<TurnoutJobState>
    {
        public TurnoutJobStateMachine(TurnoutOptions options)
        {
            Event(() => JobSubmitted, x => x.CorrelateById(m => m.Message.JobId));

            Event(() => AttemptStarted, x => x.CorrelateById(m => m.Message.JobId));
            Event(() => AttemptCompleted, x => x.CorrelateById(m => m.Message.JobId));
            Event(() => AttemptCanceled, x => x.CorrelateById(m => m.Message.JobId));
            Event(() => AttemptFaulted, x => x.CorrelateById(m => m.Message.JobId));

            Schedule(() => JobSlotWaitElapsed, instance => instance.JobSlotWaitToken, x =>
            {
                x.Delay = options.JobSlotWaitTime;
                x.Received = r => r.CorrelateById(context => context.Message.JobId);
            });

            Request(() => RequestStartJob, instance => instance.StartJobRequestId, x =>
            {
                x.Timeout = TimeSpan.FromSeconds(120);
                x.ServiceAddress = options.JobAttemptSagaEndpointAddress;
            });

            Request(() => RequestJobSlot, instance => instance.JobSlotRequestId, x =>
            {
                x.ServiceAddress = options.JobTypeSagaEndpointAddress;
                x.Timeout = TimeSpan.FromSeconds(10);
            });

            InstanceState(x => x.CurrentState, Submitted, WaitingToStart, WaitingForSlot, Started, Completed, Faulted, Canceled,
                RequestStartJob.Pending, RequestJobSlot.Pending);

            Initially(
                When(JobSubmitted)
                    .Then(OnJobSubmitted)
                    .Request(RequestJobSlot, context => CreateAllocateJobSlotRequest(context))
                    .TransitionTo(RequestJobSlot.Pending));

            During(RequestJobSlot.Pending,
                When(RequestJobSlot.Completed)
                    .Request(RequestStartJob, context => context.Init<StartJobAttempt>(new
                    {
                        context.Data.JobId,
                        context.Instance.AttemptId,
                        context.Instance.ServiceAddress,
                        Job = SerializerCache.ConvertStringToDictionary(context.Instance.JobJson)
                    }))
                    .TransitionTo(RequestStartJob.Pending),
                When(RequestJobSlot.Completed2)
                    .Schedule(JobSlotWaitElapsed, context => context.Init<JobSlotWaitElapsed>(new {JobId = context.Instance.CorrelationId}))
                    .TransitionTo(WaitingForSlot));

            During(WaitingForSlot,
                When(JobSlotWaitElapsed.Received)
                    .Request(RequestJobSlot, context => CreateAllocateJobSlotRequest(context))
                    .TransitionTo(RequestJobSlot.Pending));

            During(RequestStartJob.Pending,
                When(RequestStartJob.Completed)
                    .If(context => context.Data.AttemptId == context.Instance.AttemptId,
                        x => x.TransitionTo(WaitingToStart)),
                When(RequestStartJob.Faulted)
                    .Then(context =>
                    {
                        context.Instance.Reason = context.Data.Exceptions.FirstOrDefault()?.Message;
                    })
                    .PublishJobFaulted()
                    .TransitionTo(Faulted),
                When(RequestStartJob.TimeoutExpired)
                    .Then(context =>
                    {
                        context.Instance.Reason = "AttemptJob request timeout";
                    })
                    .PublishJobFaulted()
                    .TransitionTo(Faulted));

            During(Started, Completed, Faulted,
                Ignore(RequestStartJob.Completed),
                Ignore(RequestStartJob.TimeoutExpired),
                Ignore(RequestStartJob.Faulted));

            During(RequestStartJob.Pending, WaitingToStart, Started,
                When(AttemptStarted)
                    .Then(context => context.Instance.Started = context.Data.Timestamp)
                    .PublishJobStarted()
                    .TransitionTo(Started));


            During(RequestStartJob.Pending, WaitingToStart, Started,
                When(AttemptCompleted)
                    .Then(context =>
                    {
                        context.Instance.Completed = context.Data.Timestamp;
                        context.Instance.Duration = context.Data.Duration;
                    })
                    .PublishJobCompleted()
                    .TransitionTo(Completed));

            During(Completed,
                When(AttemptCompleted)
                    .PublishJobCompleted());


            During(RequestStartJob.Pending, WaitingToStart, Started,
                When(AttemptFaulted)
                    .Then(context =>
                    {
                        context.Instance.Faulted = context.Data.Timestamp;
                    })
                    .PublishJobFaulted()
                    .TransitionTo(Completed));

            During(Completed,
                When(AttemptFaulted)
                    .PublishJobFaulted());


            During(RequestStartJob.Pending, WaitingToStart, Started,
                When(AttemptCanceled)
                    .Then(context =>
                    {
                        context.Instance.Faulted = context.Data.Timestamp;
                    })
                    .PublishJobCanceled()
                    .TransitionTo(Canceled));

            During(Canceled,
                When(AttemptCanceled)
                    .PublishJobCanceled());

            WhenEnter(Completed, x => x.SendJobSlotReleased(options.JobTypeSagaEndpointAddress));
            WhenEnter(Canceled, x => x.SendJobSlotReleased(options.JobTypeSagaEndpointAddress));
            WhenEnter(Faulted, x => x.SendJobSlotReleased(options.JobTypeSagaEndpointAddress));
        }

        public State Submitted { get; private set; }
        public State WaitingToStart { get; private set; }
        public State WaitingForSlot { get; private set; }
        public State Started { get; private set; }
        public State Completed { get; private set; }
        public State Canceled { get; private set; }
        public State Faulted { get; private set; }

        public Event<JobSubmitted> JobSubmitted { get; private set; }

        public Event<JobAttemptStarted> AttemptStarted { get; private set; }
        public Event<JobAttemptCompleted> AttemptCompleted { get; private set; }
        public Event<JobAttemptCanceled> AttemptCanceled { get; private set; }
        public Event<JobAttemptFaulted> AttemptFaulted { get; private set; }

        public Schedule<TurnoutJobState, JobSlotWaitElapsed> JobSlotWaitElapsed { get; private set; }

        public Request<TurnoutJobState, AllocateJobSlot, JobSlotAllocated, JobSlotUnavailable> RequestJobSlot { get; private set; }
        public Request<TurnoutJobState, StartJobAttempt, JobAttemptCreated> RequestStartJob { get; private set; }

        static void OnJobSubmitted(BehaviorContext<TurnoutJobState, JobSubmitted> context)
        {
            context.Instance.Submitted = context.Data.Timestamp;

            context.Instance.JobJson = SerializerCache.ConvertDictionaryToString(context.Data.Job);
            context.Instance.ServiceAddress = context.GetPayload<ConsumeContext>().SourceAddress;
            context.Instance.JobTimeout = context.Data.JobTimeout;
            context.Instance.JobTypeId = context.Data.JobTypeId;

            context.Instance.AttemptId = NewId.NextGuid();
        }

        Task<AllocateJobSlot> CreateAllocateJobSlotRequest<T>(ConsumeEventContext<TurnoutJobState, T> context)
            where T : class
        {
            return context.Init<AllocateJobSlot>(new
            {
                JobId = context.Instance.CorrelationId,
                context.Instance.JobTypeId,
                context.Instance.JobTimeout
            });
        }
    }


    static class TurnoutJobStateMachineBehaviorExtensions
    {
        public static EventActivityBinder<TurnoutJobState> SendJobSlotReleased(this EventActivityBinder<TurnoutJobState> binder, Uri destinationAddress)
        {
            return binder.SendAsync(destinationAddress, context => context.Init<JobSlotReleased>(new
            {
                JobId = context.Instance.CorrelationId,
                context.Instance.JobTypeId
            }));
        }

        public static EventActivityBinder<TurnoutJobState, JobAttemptStarted> PublishJobStarted(
            this EventActivityBinder<TurnoutJobState, JobAttemptStarted> binder)
        {
            return binder.PublishAsync(context => context.Init<JobStarted>(new
            {
                context.Data.JobId,
                context.Data.AttemptId,
                context.Data.RetryAttempt,
                context.Data.Timestamp
            }));
        }

        public static EventActivityBinder<TurnoutJobState, JobAttemptCompleted> PublishJobCompleted(
            this EventActivityBinder<TurnoutJobState, JobAttemptCompleted> binder)
        {
            return binder.PublishAsync(context => context.Init<JobCompleted>(new
            {
                context.Data.JobId,
                context.Data.Timestamp,
                context.Data.Duration,
                context.Data.Job
            }));
        }

        public static EventActivityBinder<TurnoutJobState, JobAttemptFaulted> PublishJobFaulted(
            this EventActivityBinder<TurnoutJobState, JobAttemptFaulted> binder)
        {
            return binder.PublishAsync(context => context.Init<JobFaulted>(new
            {
                context.Data.JobId,
                context.Data.Exceptions,
                context.Data.Timestamp,
                context.Data.Job
            }));
        }

        public static EventActivityBinder<TurnoutJobState, JobAttemptCanceled> PublishJobCanceled(
            this EventActivityBinder<TurnoutJobState, JobAttemptCanceled> binder)
        {
            return binder.PublishAsync(context => context.Init<JobCanceled>(new
            {
                context.Data.JobId,
                context.Data.Timestamp
            }));
        }

        public static EventActivityBinder<TurnoutJobState, RequestTimeoutExpired<StartJobAttempt>> PublishJobFaulted(
            this EventActivityBinder<TurnoutJobState, RequestTimeoutExpired<StartJobAttempt>> binder)
        {
            return binder.PublishAsync(context => context.Init<JobFaulted>(new
            {
                JobId = context.Instance.CorrelationId,
                Exceptions = new TimeoutException(),
                InVar.Timestamp,
                Job = SerializerCache.ConvertStringToDictionary(context.Instance.JobJson)
            }));
        }

        public static EventActivityBinder<TurnoutJobState, Fault<StartJobAttempt>> PublishJobFaulted(
            this EventActivityBinder<TurnoutJobState, Fault<StartJobAttempt>> binder)
        {
            return binder.PublishAsync(context => context.Init<JobFaulted>(new
            {
                context.Data.Message.JobId,
                context.Data.Message.Job,
                context.Data.Timestamp,
                context.Data.Exceptions
            }));
        }
    }
}

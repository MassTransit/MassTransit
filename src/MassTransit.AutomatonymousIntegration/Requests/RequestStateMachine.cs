namespace Automatonymous.Requests
{
    using Activities;
    using Contracts;
    using GreenPipes;
    using MassTransit;


    /// <summary>
    /// Tracks a request, which was sent to a saga, and the saga deferred until some operation
    /// is completed, after which it will produce an event to trigger the response.
    /// </summary>
    public class RequestStateMachine :
        MassTransitStateMachine<RequestState>
    {
        public RequestStateMachine()
        {
            InstanceState(x => x.CurrentState, Pending);

            Event(() => Started, x =>
            {
                x.CorrelateById(m => m.Message.RequestId);
                x.SelectId(m => m.Message.RequestId);
            });

            Event(() => Completed, x =>
            {
                x.CorrelateById(m => m.SagaCorrelationId, i => i.Message.CorrelationId);
            });

            Event(() => Faulted, x =>
            {
                x.CorrelateById(m => m.SagaCorrelationId, i => i.Message.CorrelationId);
            });

            Initially(
                When(Started)
                    .Then(InitializeInstance)
                    .TransitionTo(Pending));

            During(Pending,
                When(Completed)
                    .Execute(x => new CompleteRequestActivity())
                    .Finalize(),
                When(Faulted)
                    .Execute(x => new FaultRequestActivity())
                    .Finalize());

            SetCompletedWhenFinalized();
        }

        static void InitializeInstance(BehaviorContext<RequestState, RequestStarted> context)
        {
            var consumeContext = context.GetPayload<ConsumeContext>();

            context.Instance.ConversationId = consumeContext.ConversationId;
            context.Instance.ResponseAddress = context.Data.ResponseAddress;
            context.Instance.FaultAddress = context.Data.FaultAddress;
            context.Instance.ExpirationTime = context.Data.ExpirationTime;

            context.Instance.SagaCorrelationId = context.Data.CorrelationId;
            context.Instance.SagaAddress = consumeContext.SourceAddress;
        }

        public State Pending { get; private set; }

        public Event<RequestStarted> Started { get; private set; }
        public Event<RequestCompleted> Completed { get; private set; }
        public Event<RequestFaulted> Faulted { get; private set; }
    }
}

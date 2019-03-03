namespace Automatonymous.Activities
{
    using System;
    using System.Threading.Tasks;
    using GreenPipes;
    using MassTransit;
    using MassTransit.Scheduling;


    public class ScheduleActivity<TInstance, TMessage> :
        Activity<TInstance>
        where TInstance : class, SagaStateMachineInstance
        where TMessage : class
    {
        readonly AsyncEventMessageFactory<TInstance, TMessage> _asyncMessageFactory;
        readonly EventMessageFactory<TInstance, TMessage> _messageFactory;
        readonly Schedule<TInstance> _schedule;
        readonly IPipe<SendContext<TMessage>> _sendPipe;
        readonly ScheduleTimeProvider<TInstance> _timeProvider;

        public ScheduleActivity(EventMessageFactory<TInstance, TMessage> messageFactory, Schedule<TInstance> schedule,
            ScheduleTimeProvider<TInstance> timeProvider, Action<SendContext<TMessage>> contextCallback)
            : this(schedule, timeProvider, contextCallback)
        {
            _messageFactory = messageFactory;
        }

        public ScheduleActivity(AsyncEventMessageFactory<TInstance, TMessage> messageFactory, Schedule<TInstance> schedule,
            ScheduleTimeProvider<TInstance> timeProvider, Action<SendContext<TMessage>> contextCallback)
            : this(schedule, timeProvider, contextCallback)
        {
            _asyncMessageFactory = messageFactory;
        }

        ScheduleActivity(Schedule<TInstance> schedule, ScheduleTimeProvider<TInstance> timeProvider, Action<SendContext<TMessage>> contextCallback)
        {
            _schedule = schedule;
            _timeProvider = timeProvider;
            _sendPipe = contextCallback != null ? Pipe.Execute(contextCallback) : Pipe.Empty<SendContext<TMessage>>();
        }

        public void Accept(StateMachineVisitor inspector)
        {
            inspector.Visit(this);
        }

        public void Probe(ProbeContext context)
        {
            var scope = context.CreateScope("schedule");
            _sendPipe.Probe(scope);
        }

        async Task Activity<TInstance>.Execute(BehaviorContext<TInstance> context, Behavior<TInstance> next)
        {
            await Execute(context).ConfigureAwait(false);

            await next.Execute(context).ConfigureAwait(false);
        }

        async Task Activity<TInstance>.Execute<T>(BehaviorContext<TInstance, T> context, Behavior<TInstance, T> next)
        {
            await Execute(context).ConfigureAwait(false);

            await next.Execute(context).ConfigureAwait(false);
        }

        Task Activity<TInstance>.Faulted<TException>(BehaviorExceptionContext<TInstance, TException> context, Behavior<TInstance> next)
        {
            return next.Faulted(context);
        }

        Task Activity<TInstance>.Faulted<T, TException>(BehaviorExceptionContext<TInstance, T, TException> context, Behavior<TInstance, T> next)
        {
            return next.Faulted(context);
        }

        async Task Execute(BehaviorContext<TInstance> context)
        {
            ConsumeEventContext<TInstance> consumeContext = context.CreateConsumeContext();

            if (!consumeContext.TryGetPayload(out MessageSchedulerContext schedulerContext))
                throw new ContextException("The scheduler context could not be retrieved.");

            var message = _messageFactory?.Invoke(consumeContext) ?? await _asyncMessageFactory(consumeContext).ConfigureAwait(false);

            var delay = _timeProvider(consumeContext);

            ScheduledMessage<TMessage> scheduledMessage = await schedulerContext.ScheduleSend(delay, message, _sendPipe).ConfigureAwait(false);

            Guid? previousTokenId = _schedule.GetTokenId(context.Instance);
            if (previousTokenId.HasValue)
                await schedulerContext.CancelScheduledSend(consumeContext.ReceiveContext.InputAddress, previousTokenId.Value).ConfigureAwait(false);

            _schedule?.SetTokenId(context.Instance, scheduledMessage.TokenId);
        }
    }


    public class ScheduleActivity<TInstance, TData, TMessage> :
        Activity<TInstance, TData>
        where TInstance : class, SagaStateMachineInstance
        where TData : class
        where TMessage : class
    {
        readonly AsyncEventMessageFactory<TInstance, TData, TMessage> _asyncMessageFactory;
        readonly EventMessageFactory<TInstance, TData, TMessage> _messageFactory;
        readonly Schedule<TInstance, TMessage> _schedule;
        readonly IPipe<SendContext<TMessage>> _sendPipe;
        readonly ScheduleTimeProvider<TInstance, TData> _timeProvider;

        public ScheduleActivity(EventMessageFactory<TInstance, TData, TMessage> messageFactory, Schedule<TInstance, TMessage> schedule,
            ScheduleTimeProvider<TInstance, TData> timeProvider, Action<SendContext<TMessage>> contextCallback)
            : this(schedule, timeProvider, contextCallback)
        {
            _messageFactory = messageFactory;
        }

        public ScheduleActivity(AsyncEventMessageFactory<TInstance, TData, TMessage> messageFactory, Schedule<TInstance, TMessage> schedule,
            ScheduleTimeProvider<TInstance, TData> timeProvider, Action<SendContext<TMessage>> contextCallback)
            : this(schedule, timeProvider, contextCallback)
        {
            _asyncMessageFactory = messageFactory;
        }

        ScheduleActivity(Schedule<TInstance, TMessage> schedule, ScheduleTimeProvider<TInstance, TData> timeProvider,
            Action<SendContext<TMessage>> contextCallback)
        {
            _schedule = schedule;
            _timeProvider = timeProvider;
            _sendPipe = contextCallback != null ? Pipe.Execute(contextCallback) : Pipe.Empty<SendContext<TMessage>>();
        }

        void Visitable.Accept(StateMachineVisitor inspector)
        {
            inspector.Visit(this);
        }

        public void Probe(ProbeContext context)
        {
            var scope = context.CreateScope("schedule");
            _sendPipe.Probe(scope);
        }

        async Task Activity<TInstance, TData>.Execute(BehaviorContext<TInstance, TData> context, Behavior<TInstance, TData> next)
        {
            ConsumeEventContext<TInstance, TData> consumeContext = context.CreateConsumeContext();

            if (!consumeContext.TryGetPayload(out MessageSchedulerContext schedulerContext))
                throw new ContextException("The scheduler context could not be retrieved.");

            var message = _messageFactory?.Invoke(consumeContext) ?? await _asyncMessageFactory(consumeContext).ConfigureAwait(false);

            var scheduledTime = _timeProvider(consumeContext);

            ScheduledMessage<TMessage> scheduledMessage = await schedulerContext.ScheduleSend(scheduledTime, message, _sendPipe).ConfigureAwait(false);

            Guid? previousTokenId = _schedule.GetTokenId(context.Instance);
            if (previousTokenId.HasValue)
                await schedulerContext.CancelScheduledSend(consumeContext.ReceiveContext.InputAddress, previousTokenId.Value).ConfigureAwait(false);

            _schedule?.SetTokenId(context.Instance, scheduledMessage.TokenId);

            await next.Execute(context).ConfigureAwait(false);
        }

        Task Activity<TInstance, TData>.Faulted<TException>(BehaviorExceptionContext<TInstance, TData, TException> context,
            Behavior<TInstance, TData> next)
        {
            return next.Faulted(context);
        }
    }
}

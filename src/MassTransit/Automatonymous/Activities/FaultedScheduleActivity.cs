namespace Automatonymous.Activities
{
    using System;
    using System.Threading.Tasks;
    using GreenPipes;
    using MassTransit;
    using MassTransit.Scheduling;

    public class FaultedScheduleActivity<TInstance, TException, TMessage> :
        Activity<TInstance>
        where TInstance : class, SagaStateMachineInstance
        where TException : Exception
        where TMessage : class
    {
        readonly AsyncEventExceptionMessageFactory<TInstance, TException, TMessage> _asyncMessageFactory;
        readonly EventExceptionMessageFactory<TInstance, TException, TMessage> _messageFactory;
        readonly Schedule<TInstance, TMessage> _schedule;
        readonly IPipe<SendContext<TMessage>> _sendPipe;
        readonly ScheduleTimeExceptionProvider<TInstance, TException> _timeProvider;

        public FaultedScheduleActivity(EventExceptionMessageFactory<TInstance, TException, TMessage> messageFactory,
            Schedule<TInstance, TMessage> schedule, ScheduleTimeExceptionProvider<TInstance, TException> timeProvider,
            Action<SendContext<TMessage>> contextCallback)
            : this(schedule, timeProvider, contextCallback)
        {
            _messageFactory = messageFactory;
        }

        public FaultedScheduleActivity(AsyncEventExceptionMessageFactory<TInstance, TException, TMessage> messageFactory,
            Schedule<TInstance, TMessage> schedule, ScheduleTimeExceptionProvider<TInstance, TException> timeProvider,
            Action<SendContext<TMessage>> contextCallback)
            : this(schedule, timeProvider, contextCallback)
        {
            _asyncMessageFactory = messageFactory;
        }

        FaultedScheduleActivity(Schedule<TInstance, TMessage> schedule, ScheduleTimeExceptionProvider<TInstance, TException> timeProvider,
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
            var scope = context.CreateScope("schedule-faulted");
            _sendPipe.Probe(scope);
        }

        Task Activity<TInstance>.Execute(BehaviorContext<TInstance> context, Behavior<TInstance> next)
        {
            return next.Execute(context);
        }

        Task Activity<TInstance>.Execute<T>(BehaviorContext<TInstance, T> context, Behavior<TInstance, T> next)
        {
            return next.Execute(context);
        }

        async Task Activity<TInstance>.Faulted<T>(BehaviorExceptionContext<TInstance, T> context, Behavior<TInstance> next)
        {
            if (context.TryGetExceptionContext(out ConsumeExceptionEventContext<TInstance, TException> exceptionContext))
            {
                if (!exceptionContext.TryGetPayload(out MessageSchedulerContext schedulerContext))
                    throw new ContextException("The scheduler context could not be retrieved.");

                var message = _messageFactory?.Invoke(exceptionContext) ?? await _asyncMessageFactory(exceptionContext).ConfigureAwait(false);

                var scheduledTime = _timeProvider(exceptionContext);

                ScheduledMessage<TMessage> scheduledMessage = await schedulerContext.ScheduleSend(scheduledTime, message, _sendPipe).ConfigureAwait(false);

                Guid? previousTokenId = _schedule.GetTokenId(context.Instance);
                if (previousTokenId.HasValue)
                    await schedulerContext.CancelScheduledSend(exceptionContext.ReceiveContext.InputAddress, previousTokenId.Value).ConfigureAwait(false);

                _schedule?.SetTokenId(context.Instance, scheduledMessage.TokenId);
            }

            await next.Faulted(context).ConfigureAwait(false);
        }

        async Task Activity<TInstance>.Faulted<T, TOtherException>(BehaviorExceptionContext<TInstance, T, TOtherException> context, Behavior<TInstance, T> next)
        {
            if (context.TryGetExceptionContext(out ConsumeExceptionEventContext<TInstance, TException> exceptionContext))
            {
                if (!exceptionContext.TryGetPayload(out MessageSchedulerContext schedulerContext))
                    throw new ContextException("The scheduler context could not be retrieved.");

                var message = _messageFactory?.Invoke(exceptionContext) ?? await _asyncMessageFactory(exceptionContext).ConfigureAwait(false);

                var scheduledTime = _timeProvider(exceptionContext);

                ScheduledMessage<TMessage> scheduledMessage = await schedulerContext.ScheduleSend(scheduledTime, message, _sendPipe).ConfigureAwait(false);

                Guid? previousTokenId = _schedule.GetTokenId(context.Instance);
                if (previousTokenId.HasValue)
                    await schedulerContext.CancelScheduledSend(exceptionContext.ReceiveContext.InputAddress, previousTokenId.Value).ConfigureAwait(false);

                _schedule?.SetTokenId(context.Instance, scheduledMessage.TokenId);
            }

            await next.Faulted(context).ConfigureAwait(false);
        }
    }

    public class FaultedScheduleActivity<TInstance, TData, TException, TMessage> :
        Activity<TInstance, TData>
        where TInstance : class, SagaStateMachineInstance
        where TData : class
        where TException : Exception
        where TMessage : class
    {
        readonly AsyncEventExceptionMessageFactory<TInstance, TData, TException, TMessage> _asyncMessageFactory;
        readonly EventExceptionMessageFactory<TInstance, TData, TException, TMessage> _messageFactory;
        readonly Schedule<TInstance, TMessage> _schedule;
        readonly IPipe<SendContext<TMessage>> _sendPipe;
        readonly ScheduleTimeExceptionProvider<TInstance, TData, TException> _timeProvider;

        public FaultedScheduleActivity(EventExceptionMessageFactory<TInstance, TData, TException, TMessage> messageFactory,
            Schedule<TInstance, TMessage> schedule, ScheduleTimeExceptionProvider<TInstance, TData, TException> timeProvider,
            Action<SendContext<TMessage>> contextCallback)
            : this(schedule, timeProvider, contextCallback)
        {
            _messageFactory = messageFactory;
        }

        public FaultedScheduleActivity(AsyncEventExceptionMessageFactory<TInstance, TData, TException, TMessage> messageFactory,
            Schedule<TInstance, TMessage> schedule, ScheduleTimeExceptionProvider<TInstance, TData, TException> timeProvider,
            Action<SendContext<TMessage>> contextCallback)
            : this(schedule, timeProvider, contextCallback)
        {
            _asyncMessageFactory = messageFactory;
        }

        FaultedScheduleActivity(Schedule<TInstance, TMessage> schedule, ScheduleTimeExceptionProvider<TInstance, TData, TException> timeProvider,
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
            var scope = context.CreateScope("schedule-faulted");
            _sendPipe.Probe(scope);
        }

        Task Activity<TInstance, TData>.Execute(BehaviorContext<TInstance, TData> context, Behavior<TInstance, TData> next)
        {
            return next.Execute(context);
        }

        async Task Activity<TInstance, TData>.Faulted<T>(BehaviorExceptionContext<TInstance, TData, T> context, Behavior<TInstance, TData> next)
        {
            if (context.TryGetExceptionContext(out ConsumeExceptionEventContext<TInstance, TData, TException> exceptionContext))
            {
                if (!exceptionContext.TryGetPayload(out MessageSchedulerContext schedulerContext))
                    throw new ContextException("The scheduler context could not be retrieved.");

                var message = _messageFactory?.Invoke(exceptionContext) ?? await _asyncMessageFactory(exceptionContext).ConfigureAwait(false);

                var scheduledTime = _timeProvider(exceptionContext);

                ScheduledMessage<TMessage> scheduledMessage = await schedulerContext.ScheduleSend(scheduledTime, message, _sendPipe).ConfigureAwait(false);

                Guid? previousTokenId = _schedule.GetTokenId(context.Instance);
                if (previousTokenId.HasValue)
                    await schedulerContext.CancelScheduledSend(exceptionContext.ReceiveContext.InputAddress, previousTokenId.Value).ConfigureAwait(false);

                _schedule?.SetTokenId(context.Instance, scheduledMessage.TokenId);
            }

            await next.Faulted(context).ConfigureAwait(false);
        }
    }
}

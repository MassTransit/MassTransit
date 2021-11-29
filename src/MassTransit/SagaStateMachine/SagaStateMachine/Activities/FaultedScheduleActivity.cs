namespace MassTransit.SagaStateMachine
{
    using System;
    using System.Threading.Tasks;


    public class FaultedScheduleActivity<TSaga, TException, TMessage> :
        IStateMachineActivity<TSaga>
        where TSaga : class, SagaStateMachineInstance
        where TException : Exception
        where TMessage : class
    {
        readonly ContextMessageFactory<BehaviorExceptionContext<TSaga, TException>, TMessage> _messageFactory;
        readonly Schedule<TSaga, TMessage> _schedule;
        readonly ScheduleTimeExceptionProvider<TSaga, TException> _timeProvider;

        public FaultedScheduleActivity(Schedule<TSaga, TMessage> schedule, ScheduleTimeExceptionProvider<TSaga, TException> timeProvider,
            ContextMessageFactory<BehaviorExceptionContext<TSaga, TException>, TMessage> messageFactory)
        {
            _messageFactory = messageFactory;
            _schedule = schedule;
            _timeProvider = timeProvider;
        }

        public void Accept(StateMachineVisitor inspector)
        {
            inspector.Visit(this);
        }

        public void Probe(ProbeContext context)
        {
            context.CreateScope("schedule-faulted");
        }

        public Task Execute(BehaviorContext<TSaga> context, IBehavior<TSaga> next)
        {
            return next.Execute(context);
        }

        public Task Execute<T>(BehaviorContext<TSaga, T> context, IBehavior<TSaga, T> next)
            where T : class
        {
            return next.Execute(context);
        }

        public async Task Faulted<T>(BehaviorExceptionContext<TSaga, T> context, IBehavior<TSaga> next)
            where T : Exception
        {
            if (context is BehaviorExceptionContext<TSaga, TException> exceptionContext)
                await Schedule(context, exceptionContext).ConfigureAwait(false);

            await next.Faulted(context).ConfigureAwait(false);
        }

        public async Task Faulted<T, TOtherException>(BehaviorExceptionContext<TSaga, T, TOtherException> context, IBehavior<TSaga, T> next)
            where T : class
            where TOtherException : Exception
        {
            if (context is BehaviorExceptionContext<TSaga, TException> exceptionContext)
                await Schedule(context, exceptionContext).ConfigureAwait(false);

            await next.Faulted(context).ConfigureAwait(false);
        }

        async Task Schedule<T>(BehaviorExceptionContext<TSaga, T> context, BehaviorExceptionContext<TSaga, TException> exceptionContext)
            where T : Exception
        {
            Guid? previousTokenId = _schedule.GetTokenId(context.Saga);

            var schedulerContext = context.GetPayload<MessageSchedulerContext>();

            ScheduledMessage<TMessage> message = await _messageFactory
                .Use(exceptionContext, (ctx, s) => schedulerContext.ScheduleSend(_timeProvider(ctx), s.Message, s.Pipe)).ConfigureAwait(false);

            _schedule?.SetTokenId(context.Saga, message.TokenId);

            if (previousTokenId.HasValue)
            {
                Guid? messageTokenId = context.GetSchedulingTokenId();
                if (!messageTokenId.HasValue || previousTokenId.Value != messageTokenId.Value)
                    await schedulerContext.CancelScheduledSend(context.ReceiveContext.InputAddress, previousTokenId.Value).ConfigureAwait(false);
            }
        }
    }


    public class FaultedScheduleActivity<TSaga, TData, TException, TMessage> :
        IStateMachineActivity<TSaga, TData>
        where TSaga : class, SagaStateMachineInstance
        where TData : class
        where TException : Exception
        where TMessage : class
    {
        readonly ContextMessageFactory<BehaviorExceptionContext<TSaga, TData, TException>, TMessage> _messageFactory;
        readonly Schedule<TSaga, TMessage> _schedule;
        readonly ScheduleTimeExceptionProvider<TSaga, TData, TException> _timeProvider;

        public FaultedScheduleActivity(Schedule<TSaga, TMessage> schedule, ScheduleTimeExceptionProvider<TSaga, TData, TException> timeProvider,
            ContextMessageFactory<BehaviorExceptionContext<TSaga, TData, TException>, TMessage> messageFactory)
        {
            _messageFactory = messageFactory;
            _schedule = schedule;
            _timeProvider = timeProvider;
        }

        public void Accept(StateMachineVisitor inspector)
        {
            inspector.Visit(this);
        }

        public void Probe(ProbeContext context)
        {
            context.CreateScope("schedule-faulted");
        }

        public Task Execute(BehaviorContext<TSaga, TData> context, IBehavior<TSaga, TData> next)
        {
            return next.Execute(context);
        }

        public async Task Faulted<T>(BehaviorExceptionContext<TSaga, TData, T> context, IBehavior<TSaga, TData> next)
            where T : Exception
        {
            if (context is BehaviorExceptionContext<TSaga, TData, TException> exceptionContext)
            {
                Guid? previousTokenId = _schedule.GetTokenId(context.Saga);

                var schedulerContext = context.GetPayload<MessageSchedulerContext>();

                ScheduledMessage<TMessage> message = await _messageFactory
                    .Use(exceptionContext, (ctx, s) => schedulerContext.ScheduleSend(_timeProvider(ctx), s.Message, s.Pipe)).ConfigureAwait(false);

                _schedule?.SetTokenId(context.Saga, message.TokenId);

                if (previousTokenId.HasValue)
                {
                    Guid? messageTokenId = context.GetSchedulingTokenId();
                    if (!messageTokenId.HasValue || previousTokenId.Value != messageTokenId.Value)
                        await schedulerContext.CancelScheduledSend(context.ReceiveContext.InputAddress, previousTokenId.Value).ConfigureAwait(false);
                }
            }

            await next.Faulted(context).ConfigureAwait(false);
        }
    }
}

namespace MassTransit.SagaStateMachine
{
    using System;
    using System.Threading.Tasks;


    public class UnscheduleActivity<TSaga> :
        IStateMachineActivity<TSaga>
        where TSaga : class, SagaStateMachineInstance
    {
        readonly Schedule<TSaga> _schedule;

        public UnscheduleActivity(Schedule<TSaga> schedule)
        {
            _schedule = schedule;
        }

        public void Accept(StateMachineVisitor inspector)
        {
            inspector.Visit(this);
        }

        public void Probe(ProbeContext context)
        {
            context.CreateScope("unschedule");
        }

        public async Task Execute(BehaviorContext<TSaga> context, IBehavior<TSaga> next)
        {
            await Execute(context).ConfigureAwait(false);

            await next.Execute(context).ConfigureAwait(false);
        }

        public async Task Execute<T>(BehaviorContext<TSaga, T> context, IBehavior<TSaga, T> next)
            where T : class
        {
            await Execute(context).ConfigureAwait(false);

            await next.Execute(context).ConfigureAwait(false);
        }

        public Task Faulted<TException>(BehaviorExceptionContext<TSaga, TException> context, IBehavior<TSaga> next)
            where TException : Exception
        {
            return next.Faulted(context);
        }

        public Task Faulted<T, TException>(BehaviorExceptionContext<TSaga, T, TException> context, IBehavior<TSaga, T> next)
            where T : class
            where TException : Exception
        {
            return next.Faulted(context);
        }

        async Task Execute(SagaConsumeContext<TSaga> context)
        {
            Guid? previousTokenId = _schedule.GetTokenId(context.Saga);
            if (previousTokenId.HasValue)
            {
                Guid? messageTokenId = context.GetSchedulingTokenId();
                if (!messageTokenId.HasValue || previousTokenId.Value != messageTokenId.Value)
                {
                    var schedulerContext = context.GetPayload<MessageSchedulerContext>();

                    await schedulerContext.CancelScheduledSend(context.ReceiveContext.InputAddress, previousTokenId.Value).ConfigureAwait(false);

                    _schedule.SetTokenId(context.Saga, default);
                }
            }
        }
    }
}

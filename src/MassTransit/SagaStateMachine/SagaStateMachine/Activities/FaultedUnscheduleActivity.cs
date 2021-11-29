namespace MassTransit.SagaStateMachine
{
    using System;
    using System.Threading.Tasks;


    public class FaultedUnscheduleActivity<TSaga> :
        IStateMachineActivity<TSaga>
        where TSaga : class, SagaStateMachineInstance
    {
        readonly Schedule<TSaga> _schedule;

        public FaultedUnscheduleActivity(Schedule<TSaga> schedule)
        {
            _schedule = schedule;
        }

        public void Accept(StateMachineVisitor inspector)
        {
            inspector.Visit(this);
        }

        public void Probe(ProbeContext context)
        {
            context.CreateScope("unschedule-faulted");
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

        public async Task Faulted<TException>(BehaviorExceptionContext<TSaga, TException> context, IBehavior<TSaga> next)
            where TException : Exception
        {
            await Faulted(context).ConfigureAwait(false);

            await next.Faulted(context).ConfigureAwait(false);
        }

        public async Task Faulted<T, TException>(BehaviorExceptionContext<TSaga, T, TException> context, IBehavior<TSaga, T> next)
            where T : class
            where TException : Exception
        {
            await Faulted(context).ConfigureAwait(false);

            await next.Faulted(context).ConfigureAwait(false);
        }

        async Task Faulted(SagaConsumeContext<TSaga> context)
        {
            var schedulerContext = context.GetPayload<MessageSchedulerContext>();

            Guid? previousTokenId = _schedule.GetTokenId(context.Saga);
            if (previousTokenId.HasValue)
            {
                await schedulerContext.CancelScheduledSend(context.ReceiveContext.InputAddress, previousTokenId.Value).ConfigureAwait(false);

                _schedule.SetTokenId(context.Saga, default);
            }
        }
    }
}

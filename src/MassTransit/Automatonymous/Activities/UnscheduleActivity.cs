namespace Automatonymous.Activities
{
    using System;
    using System.Threading.Tasks;
    using GreenPipes;
    using MassTransit;


    public class UnscheduleActivity<TInstance> :
        Activity<TInstance>
        where TInstance : class, SagaStateMachineInstance
    {
        readonly Schedule<TInstance> _schedule;

        public UnscheduleActivity(Schedule<TInstance> schedule)
        {
            _schedule = schedule;
        }

        public void Accept(StateMachineVisitor inspector)
        {
            inspector.Visit(this);
        }

        public void Probe(ProbeContext context)
        {
            var scope = context.CreateScope("unschedule");
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
            if (!context.TryGetPayload(out ConsumeContext consumeContext))
                throw new ContextException("The consume context could not be retrieved.");

            if (!consumeContext.TryGetPayload(out MessageSchedulerContext schedulerContext))
                throw new ContextException("The scheduler context could not be retrieved.");

            Guid? previousTokenId = _schedule.GetTokenId(context.Instance);
            if (previousTokenId.HasValue)
            {
                await schedulerContext.CancelScheduledSend(consumeContext.ReceiveContext.InputAddress, previousTokenId.Value).ConfigureAwait(false);

                _schedule.SetTokenId(context.Instance, default);
            }
        }
    }
}

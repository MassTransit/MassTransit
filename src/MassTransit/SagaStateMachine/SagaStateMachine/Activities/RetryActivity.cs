namespace MassTransit.SagaStateMachine
{
    using System;
    using System.Threading.Tasks;
    using RetryPolicies;


    public class RetryActivity<TInstance> :
        IStateMachineActivity<TInstance>
        where TInstance : class, ISaga
    {
        readonly IBehavior<TInstance> _retryBehavior;
        readonly IRetryPolicy _retryPolicy;

        public RetryActivity(IRetryPolicy retryPolicy, IBehavior<TInstance> retryBehavior)
        {
            _retryPolicy = retryPolicy;
            _retryBehavior = retryBehavior;
        }

        public void Probe(ProbeContext context)
        {
            var scope = context.CreateScope("retry");

            _retryBehavior.Probe(scope);
        }

        public void Accept(StateMachineVisitor visitor)
        {
            visitor.Visit(this, x => _retryBehavior.Accept(visitor));
        }

        public async Task Execute(BehaviorContext<TInstance> context, IBehavior<TInstance> next)
        {
            await _retryPolicy.Retry(() => _retryBehavior.Execute(context), context.CancellationToken);

            await next.Execute(context).ConfigureAwait(false);
        }

        public async Task Execute<T>(BehaviorContext<TInstance, T> context, IBehavior<TInstance, T> next)
            where T : class
        {
            await _retryPolicy.Retry(() => _retryBehavior.Execute(context), context.CancellationToken);

            await next.Execute(context).ConfigureAwait(false);
        }

        public Task Faulted<TException>(BehaviorExceptionContext<TInstance, TException> context, IBehavior<TInstance> next)
            where TException : Exception
        {
            return next.Faulted(context);
        }

        public Task Faulted<T, TException>(BehaviorExceptionContext<TInstance, T, TException> context, IBehavior<TInstance, T> next)
            where T : class
            where TException : Exception
        {
            return next.Faulted(context);
        }
    }


    public class RetryActivity<TInstance, TMessage> :
        IStateMachineActivity<TInstance>
        where TInstance : class, ISaga
        where TMessage : class
    {
        readonly IBehavior<TInstance> _retryBehavior;
        readonly IRetryPolicy _retryPolicy;

        public RetryActivity(IRetryPolicy retryPolicy, IBehavior<TInstance> retryBehavior)
        {
            _retryPolicy = retryPolicy;
            _retryBehavior = retryBehavior;
        }

        public void Probe(ProbeContext context)
        {
            var scope = context.CreateScope("retry");

            _retryBehavior.Probe(scope);
        }

        public void Accept(StateMachineVisitor visitor)
        {
            visitor.Visit(this, x => _retryBehavior.Accept(visitor));
        }

        public Task Execute(BehaviorContext<TInstance> context, IBehavior<TInstance> next)
        {
            throw new SagaStateMachineException("This activity requires a body with the event, but no body was specified.");
        }

        public async Task Execute<T>(BehaviorContext<TInstance, T> context, IBehavior<TInstance, T> next)
            where T : class
        {
            if (context is BehaviorContext<TInstance, TMessage> behaviorContext)
                await _retryPolicy.Retry(() => _retryBehavior.Execute(behaviorContext), context.CancellationToken);

            await next.Execute(context).ConfigureAwait(false);
        }

        public Task Faulted<TException>(BehaviorExceptionContext<TInstance, TException> context, IBehavior<TInstance> next)
            where TException : Exception
        {
            return next.Faulted(context);
        }

        public Task Faulted<T, TException>(BehaviorExceptionContext<TInstance, T, TException> context, IBehavior<TInstance, T> next)
            where T : class
            where TException : Exception
        {
            return next.Faulted(context);
        }
    }
}

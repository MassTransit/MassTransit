namespace MassTransit.SagaStateMachine
{
    using System;
    using System.Threading.Tasks;


    public class ConditionExceptionActivity<TSaga, TConditionException> :
        IStateMachineActivity<TSaga>
        where TSaga : class, ISaga
        where TConditionException : Exception
    {
        readonly StateMachineAsyncExceptionCondition<TSaga, TConditionException> _condition;
        readonly IBehavior<TSaga> _elseBehavior;
        readonly IBehavior<TSaga> _thenBehavior;

        public ConditionExceptionActivity(StateMachineAsyncExceptionCondition<TSaga, TConditionException> condition, IBehavior<TSaga> thenBehavior,
            IBehavior<TSaga> elseBehavior)
        {
            _condition = condition;
            _thenBehavior = thenBehavior;
            _elseBehavior = elseBehavior;
        }

        void IProbeSite.Probe(ProbeContext context)
        {
            var scope = context.CreateScope("condition");

            _thenBehavior.Probe(scope);
            _elseBehavior.Probe(scope);
        }

        public void Accept(StateMachineVisitor visitor)
        {
            visitor.Visit(this, x => _thenBehavior.Accept(visitor));
            visitor.Visit(this, x => _elseBehavior.Accept(visitor));
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
            var behaviorContext = context as BehaviorExceptionContext<TSaga, TConditionException>;
            if (behaviorContext != null)
            {
                if (await _condition(behaviorContext).ConfigureAwait(false))
                    await _thenBehavior.Faulted(context).ConfigureAwait(false);
                else
                    await _elseBehavior.Faulted(context).ConfigureAwait(false);
            }

            await next.Faulted(context).ConfigureAwait(false);
        }

        public async Task Faulted<T, TException>(BehaviorExceptionContext<TSaga, T, TException> context, IBehavior<TSaga, T> next)
            where T : class
            where TException : Exception
        {
            var behaviorContext = context as BehaviorExceptionContext<TSaga, T, TConditionException>;
            if (behaviorContext != null)
            {
                if (await _condition(behaviorContext).ConfigureAwait(false))
                    await _thenBehavior.Faulted(context).ConfigureAwait(false);
                else
                    await _elseBehavior.Faulted(context).ConfigureAwait(false);
            }

            await next.Faulted(context).ConfigureAwait(false);
        }
    }


    public class ConditionExceptionActivity<TSaga, TMessage, TConditionException> :
        IStateMachineActivity<TSaga>
        where TSaga : class, ISaga
        where TMessage : class
        where TConditionException : Exception
    {
        readonly StateMachineAsyncExceptionCondition<TSaga, TMessage, TConditionException> _condition;
        readonly IBehavior<TSaga> _elseBehavior;
        readonly IBehavior<TSaga> _thenBehavior;

        public ConditionExceptionActivity(StateMachineAsyncExceptionCondition<TSaga, TMessage, TConditionException> condition,
            IBehavior<TSaga> thenBehavior, IBehavior<TSaga> elseBehavior)
        {
            _condition = condition;
            _thenBehavior = thenBehavior;
            _elseBehavior = elseBehavior;
        }

        void IProbeSite.Probe(ProbeContext context)
        {
            var scope = context.CreateScope("condition");

            _thenBehavior.Probe(scope);
            _elseBehavior.Probe(scope);
        }

        public void Accept(StateMachineVisitor visitor)
        {
            visitor.Visit(this, x => _thenBehavior.Accept(visitor));
            visitor.Visit(this, x => _elseBehavior.Accept(visitor));
        }

        public Task Execute(BehaviorContext<TSaga> context, IBehavior<TSaga> next)
        {
            throw new SagaStateMachineException("This activity requires a body with the event, but no body was specified.");
        }

        public Task Execute<T>(BehaviorContext<TSaga, T> context, IBehavior<TSaga, T> next)
            where T : class
        {
            return next.Execute(context);
        }

        public Task Faulted<TException>(BehaviorExceptionContext<TSaga, TException> context, IBehavior<TSaga> next)
            where TException : Exception
        {
            throw new SagaStateMachineException("This activity requires a body with the event, but no body was specified.");
        }

        public async Task Faulted<T, TException>(BehaviorExceptionContext<TSaga, T, TException> context, IBehavior<TSaga, T> next)
            where T : class
            where TException : Exception
        {
            var behaviorContext = context as BehaviorExceptionContext<TSaga, TMessage, TConditionException>;
            if (behaviorContext != null)
            {
                if (await _condition(behaviorContext).ConfigureAwait(false))
                    await _thenBehavior.Faulted(context).ConfigureAwait(false);
                else
                    await _elseBehavior.Faulted(context).ConfigureAwait(false);
            }

            await next.Faulted(context).ConfigureAwait(false);
        }
    }
}

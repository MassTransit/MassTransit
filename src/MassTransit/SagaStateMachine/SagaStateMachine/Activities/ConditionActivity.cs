namespace MassTransit.SagaStateMachine
{
    using System;
    using System.Threading.Tasks;


    public class ConditionActivity<TSaga> :
        IStateMachineActivity<TSaga>
        where TSaga : class, ISaga
    {
        readonly StateMachineAsyncCondition<TSaga> _condition;
        readonly IBehavior<TSaga> _elseBehavior;
        readonly IBehavior<TSaga> _thenBehavior;

        public ConditionActivity(StateMachineAsyncCondition<TSaga> condition, IBehavior<TSaga> thenBehavior, IBehavior<TSaga> elseBehavior)
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

        public async Task Execute(BehaviorContext<TSaga> context, IBehavior<TSaga> next)
        {
            if (await _condition(context).ConfigureAwait(false))
                await _thenBehavior.Execute(context).ConfigureAwait(false);
            else
                await _elseBehavior.Execute(context).ConfigureAwait(false);

            await next.Execute(context).ConfigureAwait(false);
        }

        public async Task Execute<T>(BehaviorContext<TSaga, T> context, IBehavior<TSaga, T> next)
            where T : class
        {
            if (await _condition(context).ConfigureAwait(false))
                await _thenBehavior.Execute(context).ConfigureAwait(false);
            else
                await _elseBehavior.Execute(context).ConfigureAwait(false);

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
    }


    public class ConditionActivity<TSaga, TMessage> :
        IStateMachineActivity<TSaga>
        where TSaga : class, ISaga
        where TMessage : class
    {
        readonly StateMachineAsyncCondition<TSaga, TMessage> _condition;
        readonly IBehavior<TSaga> _elseBehavior;
        readonly IBehavior<TSaga> _thenBehavior;

        public ConditionActivity(StateMachineAsyncCondition<TSaga, TMessage> condition, IBehavior<TSaga> thenBehavior, IBehavior<TSaga> elseBehavior)
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

        public async Task Execute<T>(BehaviorContext<TSaga, T> context, IBehavior<TSaga, T> next)
            where T : class
        {
            if (context is BehaviorContext<TSaga, TMessage> behaviorContext)
            {
                if (await _condition(behaviorContext).ConfigureAwait(false))
                    await _thenBehavior.Execute(behaviorContext).ConfigureAwait(false);
                else
                    await _elseBehavior.Execute(behaviorContext).ConfigureAwait(false);
            }

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
    }
}

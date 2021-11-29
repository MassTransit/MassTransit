namespace MassTransit.SagaStateMachine
{
    using System;
    using System.Threading.Tasks;


    /// <summary>
    /// Catches an exception of a specific type and compensates using the behavior
    /// </summary>
    /// <typeparam name="TSaga"></typeparam>
    /// <typeparam name="TException"></typeparam>
    public class CatchFaultActivity<TSaga, TException> :
        IStateMachineActivity<TSaga>
        where TSaga : class, ISaga
        where TException : Exception
    {
        readonly IBehavior<TSaga> _behavior;

        public CatchFaultActivity(IBehavior<TSaga> behavior)
        {
            _behavior = behavior;
        }

        public void Accept(StateMachineVisitor visitor)
        {
            visitor.Visit(this, x => _behavior.Accept(visitor));
        }

        public void Probe(ProbeContext context)
        {
            var scope = context.CreateScope("catch");

            scope.Add("exceptionType", TypeCache<TException>.ShortName);

            _behavior.Probe(scope.CreateScope("behavior"));
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
            {
                await _behavior.Faulted(exceptionContext).ConfigureAwait(false);

                // if the compensate returns, we should go forward normally
                await next.Execute(context).ConfigureAwait(false);
            }
            else
                await next.Faulted(context).ConfigureAwait(false);
        }

        public async Task Faulted<TMessage, T>(BehaviorExceptionContext<TSaga, TMessage, T> context, IBehavior<TSaga, TMessage> next)
            where TMessage : class
            where T : Exception
        {
            if (context is BehaviorExceptionContext<TSaga, TMessage, TException> exceptionContext)
            {
                await _behavior.Faulted(exceptionContext).ConfigureAwait(false);

                // if the compensate returns, we should go forward normally
                await next.Execute(context).ConfigureAwait(false);
            }
            else
                await next.Faulted(context).ConfigureAwait(false);
        }
    }
}

namespace MassTransit.SagaStateMachine
{
    using System;
    using System.Threading.Tasks;


    public class AsyncActivity<TSaga> :
        IStateMachineActivity<TSaga>
        where TSaga : class, ISaga
    {
        readonly Func<BehaviorContext<TSaga>, Task> _asyncAction;

        public AsyncActivity(Func<BehaviorContext<TSaga>, Task> asyncAction)
        {
            _asyncAction = asyncAction;
        }

        public void Accept(StateMachineVisitor visitor)
        {
            visitor.Visit(this);
        }

        public void Probe(ProbeContext context)
        {
            context.CreateScope("thenAsync");
        }

        public async Task Execute(BehaviorContext<TSaga> context, IBehavior<TSaga> next)
        {
            await _asyncAction(context).ConfigureAwait(false);

            await next.Execute(context).ConfigureAwait(false);
        }

        public async Task Execute<TData>(BehaviorContext<TSaga, TData> context, IBehavior<TSaga, TData> next)
            where TData : class
        {
            await _asyncAction(context).ConfigureAwait(false);

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


    public class AsyncActivity<TInstance, TData> :
        IStateMachineActivity<TInstance, TData>
        where TInstance : class, ISaga
        where TData : class
    {
        readonly Func<BehaviorContext<TInstance, TData>, Task> _asyncAction;

        public AsyncActivity(Func<BehaviorContext<TInstance, TData>, Task> asyncAction)
        {
            _asyncAction = asyncAction;
        }

        public void Accept(StateMachineVisitor visitor)
        {
            visitor.Visit(this);
        }

        public void Probe(ProbeContext context)
        {
            context.CreateScope("thenAsync");
        }

        public async Task Execute(BehaviorContext<TInstance, TData> context, IBehavior<TInstance, TData> next)
        {
            await _asyncAction(context).ConfigureAwait(false);

            await next.Execute(context).ConfigureAwait(false);
        }

        public Task Faulted<TException>(BehaviorExceptionContext<TInstance, TData, TException> context, IBehavior<TInstance, TData> next)
            where TException : Exception
        {
            return next.Faulted(context);
        }
    }
}

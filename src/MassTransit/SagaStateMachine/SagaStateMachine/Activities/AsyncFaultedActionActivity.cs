namespace MassTransit.SagaStateMachine
{
    using System;
    using System.Threading.Tasks;


    public class AsyncFaultedActionActivity<TSaga, TException> :
        IStateMachineActivity<TSaga>
        where TException : Exception
        where TSaga : class, ISaga
    {
        readonly Func<BehaviorExceptionContext<TSaga, TException>, Task> _asyncAction;

        public AsyncFaultedActionActivity(Func<BehaviorExceptionContext<TSaga, TException>, Task> asyncAction)
        {
            _asyncAction = asyncAction;
        }

        public void Accept(StateMachineVisitor visitor)
        {
            visitor.Visit(this);
        }

        public void Probe(ProbeContext context)
        {
            context.CreateScope("then-async-faulted");
        }

        public Task Execute(BehaviorContext<TSaga> context, IBehavior<TSaga> next)
        {
            return next.Execute(context);
        }

        public Task Execute<TData>(BehaviorContext<TSaga, TData> context, IBehavior<TSaga, TData> next)
            where TData : class
        {
            return next.Execute(context);
        }

        public async Task Faulted<T>(BehaviorExceptionContext<TSaga, T> context, IBehavior<TSaga> next)
            where T : Exception
        {
            var exceptionContext = context as BehaviorExceptionContext<TSaga, TException>;
            if (exceptionContext != null)
                await _asyncAction(exceptionContext);

            await next.Faulted(context);
        }

        public async Task Faulted<TData, T>(BehaviorExceptionContext<TSaga, TData, T> context, IBehavior<TSaga, TData> next)
            where TData : class
            where T : Exception
        {
            var exceptionContext = context as BehaviorExceptionContext<TSaga, TData, TException>;
            if (exceptionContext != null)
                await _asyncAction(exceptionContext);

            await next.Faulted(context);
        }
    }


    public class AsyncFaultedActionActivity<TSaga, TMessage, TException> :
        IStateMachineActivity<TSaga, TMessage>
        where TSaga : class, ISaga
        where TException : Exception
        where TMessage : class
    {
        readonly Func<BehaviorExceptionContext<TSaga, TMessage, TException>, Task> _asyncAction;

        public AsyncFaultedActionActivity(Func<BehaviorExceptionContext<TSaga, TMessage, TException>, Task> asyncAction)
        {
            _asyncAction = asyncAction;
        }

        public void Accept(StateMachineVisitor visitor)
        {
            visitor.Visit(this);
        }

        public void Probe(ProbeContext context)
        {
            context.CreateScope("then-async-faulted");
        }

        public Task Execute(BehaviorContext<TSaga, TMessage> context, IBehavior<TSaga, TMessage> next)
        {
            return next.Execute(context);
        }

        public async Task Faulted<T>(BehaviorExceptionContext<TSaga, TMessage, T> context, IBehavior<TSaga, TMessage> next)
            where T : Exception
        {
            var exceptionContext = context as BehaviorExceptionContext<TSaga, TMessage, TException>;
            if (exceptionContext != null)
                await _asyncAction(exceptionContext);

            await next.Faulted(context);
        }
    }
}

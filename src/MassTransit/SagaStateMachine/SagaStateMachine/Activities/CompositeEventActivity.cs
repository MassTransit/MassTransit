namespace MassTransit.SagaStateMachine
{
    using System;
    using System.Threading.Tasks;


    public class CompositeEventActivity<TSaga> :
        IStateMachineActivity<TSaga>
        where TSaga : class, ISaga
    {
        readonly ICompositeEventStatusAccessor<TSaga> _accessor;
        readonly CompositeEventStatus _complete;
        readonly int _flag;

        public CompositeEventActivity(ICompositeEventStatusAccessor<TSaga> accessor, int flag, CompositeEventStatus complete, Event @event)
        {
            _accessor = accessor;
            _flag = flag;
            _complete = complete;
            Event = @event;
        }

        public Event Event { get; }

        public void Accept(StateMachineVisitor visitor)
        {
            visitor.Visit(this);
        }

        public void Probe(ProbeContext context)
        {
            var scope = context.CreateScope("compositeEvent");
            _accessor.Probe(scope);
            scope.Add("event", Event.Name);
            scope.Add("flag", _flag.ToString("X8"));
        }

        public async Task Execute(BehaviorContext<TSaga> context, IBehavior<TSaga> next)
        {
            await Execute(context).ConfigureAwait(false);

            await next.Execute(context).ConfigureAwait(false);
        }

        public async Task Execute<TData>(BehaviorContext<TSaga, TData> context, IBehavior<TSaga, TData> next)
            where TData : class
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

        Task Execute(BehaviorContext<TSaga> context)
        {
            var value = _accessor.Get(context.Saga);
            value.Set(_flag);

            _accessor.Set(context.Saga, value);

            return value.Equals(_complete)
                ? RaiseCompositeEvent(context)
                : Task.CompletedTask;
        }

        Task RaiseCompositeEvent(BehaviorContext<TSaga> context)
        {
            return context.Raise(Event);
        }
    }
}

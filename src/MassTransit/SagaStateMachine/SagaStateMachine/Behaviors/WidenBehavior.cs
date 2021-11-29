namespace MassTransit.SagaStateMachine
{
    using System.Threading.Tasks;


    public class WidenBehavior<TSaga, TMessage> :
        IBehavior<TSaga>
        where TSaga : class, ISaga
        where TMessage : class
    {
        readonly Event<TMessage> _event;
        readonly TMessage _message;
        readonly IBehavior<TSaga, TMessage> _next;

        public WidenBehavior(IBehavior<TSaga, TMessage> next, BehaviorContext<TSaga, TMessage> context)
        {
            _next = next;
            _message = context.Message;
            _event = context.Event;
        }

        public void Accept(StateMachineVisitor visitor)
        {
            visitor.Visit(this);
        }

        public void Probe(ProbeContext context)
        {
            _next.Probe(context);
        }

        Task IBehavior<TSaga>.Execute(BehaviorContext<TSaga> context)
        {
            BehaviorContext<TSaga, TMessage> nextContext = context.CreateProxy(_event, _message);

            return _next.Execute(nextContext);
        }

        Task IBehavior<TSaga>.Execute<T>(BehaviorContext<TSaga, T> context)
        {
            BehaviorContext<TSaga, TMessage> nextContext = context as BehaviorContext<TSaga, TMessage> ?? context.CreateProxy(_event, _message);

            return _next.Execute(nextContext);
        }

        Task IBehavior<TSaga>.Faulted<T, TException>(BehaviorExceptionContext<TSaga, T, TException> context)
        {
            BehaviorExceptionContext<TSaga, TMessage, TException> nextContext =
                context as BehaviorExceptionContext<TSaga, TMessage, TException> ?? context.CreateProxy(_event, _message);

            return _next.Faulted(nextContext);
        }

        Task IBehavior<TSaga>.Faulted<TException>(BehaviorExceptionContext<TSaga, TException> context)
        {
            BehaviorExceptionContext<TSaga, TMessage, TException> nextContext = context.CreateProxy(_event, _message);

            return _next.Faulted(nextContext);
        }
    }
}

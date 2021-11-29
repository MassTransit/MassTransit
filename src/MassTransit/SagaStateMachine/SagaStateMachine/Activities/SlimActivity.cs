namespace MassTransit.SagaStateMachine
{
    using System.Threading.Tasks;


    /// <summary>
    /// Adapts an Activity to a Data Activity context
    /// </summary>
    /// <typeparam name="TSaga"></typeparam>
    /// <typeparam name="TMessage"></typeparam>
    public class SlimActivity<TSaga, TMessage> :
        IStateMachineActivity<TSaga, TMessage>
        where TSaga : class, ISaga
        where TMessage : class
    {
        readonly IStateMachineActivity<TSaga> _activity;

        public SlimActivity(IStateMachineActivity<TSaga> activity)
        {
            _activity = activity;
        }

        public void Accept(StateMachineVisitor visitor)
        {
            _activity.Accept(visitor);
        }

        public void Probe(ProbeContext context)
        {
            _activity.Probe(context);
        }

        Task IStateMachineActivity<TSaga, TMessage>.Execute(BehaviorContext<TSaga, TMessage> context, IBehavior<TSaga, TMessage> behavior)
        {
            return _activity.Execute(context, new WidenBehavior<TSaga, TMessage>(behavior, context));
        }

        Task IStateMachineActivity<TSaga, TMessage>.Faulted<TException>(BehaviorExceptionContext<TSaga, TMessage, TException> context, IBehavior<TSaga, TMessage> next)
        {
            return _activity.Faulted(context, next);
        }
    }
}

namespace MassTransit.SagaStateMachine
{
    using System.Threading.Tasks;


    /// <summary>
    /// Splits apart the data from the behavior so it can be invoked properly.
    /// </summary>
    /// <typeparam name="TSaga">The instance type</typeparam>
    /// <typeparam name="TMessage">The event data type</typeparam>
    public class DataBehavior<TSaga, TMessage> :
        IBehavior<TSaga, TMessage>
        where TSaga : class, ISaga
        where TMessage : class
    {
        readonly IBehavior<TSaga> _behavior;

        public DataBehavior(IBehavior<TSaga> behavior)
        {
            _behavior = behavior;
        }

        public void Accept(StateMachineVisitor visitor)
        {
            _behavior.Accept(visitor);
        }

        public void Probe(ProbeContext context)
        {
            _behavior.Probe(context);
        }

        Task IBehavior<TSaga, TMessage>.Execute(BehaviorContext<TSaga, TMessage> context)
        {
            return _behavior.Execute(context);
        }

        Task IBehavior<TSaga, TMessage>.Faulted<TException>(BehaviorExceptionContext<TSaga, TMessage, TException> context)
        {
            return _behavior.Faulted(context);
        }
    }
}

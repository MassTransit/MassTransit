namespace MassTransit.SagaStateMachine
{
    using System;
    using System.Threading.Tasks;


    public class DataConverterActivity<TSaga, TMessage> :
        IStateMachineActivity<TSaga>
        where TSaga : class, SagaStateMachineInstance
        where TMessage : class
    {
        readonly IStateMachineActivity<TSaga, TMessage> _activity;

        public DataConverterActivity(IStateMachineActivity<TSaga, TMessage> activity)
        {
            _activity = activity;
        }

        public void Accept(StateMachineVisitor visitor)
        {
            visitor.Visit(this, _ => _activity.Accept(visitor));
        }

        public void Probe(ProbeContext context)
        {
            _activity.Probe(context);
        }

        public Task Execute(BehaviorContext<TSaga> context, IBehavior<TSaga> next)
        {
            throw new SagaStateMachineException("This activity requires a body with the event, but no body was specified.");
        }

        public Task Execute<T>(BehaviorContext<TSaga, T> context, IBehavior<TSaga, T> next)
            where T : class
        {
            if (context is not BehaviorContext<TSaga, TMessage> dataContext)
                throw new SagaStateMachineException("Expected Type " + typeof(TMessage).Name + " but was " + context.Message.GetType().Name);

            if (next is not IBehavior<TSaga, TMessage> dataNext)
                throw new SagaStateMachineException("The next behavior was not a valid type");

            return _activity.Execute(dataContext, dataNext);
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
            if (context is not BehaviorExceptionContext<TSaga, TMessage, TException> dataContext)
                throw new SagaStateMachineException("Expected Type " + typeof(TMessage).Name + " but was " + context.Message.GetType().Name);

            if (next is not IBehavior<TSaga, TMessage> dataNext)
                throw new SagaStateMachineException("The next behavior was not a valid type");

            return _activity.Faulted(dataContext, dataNext);
        }
    }
}

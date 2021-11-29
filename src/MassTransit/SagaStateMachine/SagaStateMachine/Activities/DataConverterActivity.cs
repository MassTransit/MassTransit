namespace MassTransit.SagaStateMachine
{
    using System;
    using System.Threading.Tasks;


    public class DataConverterActivity<TSaga, TMessage> :
        IStateMachineActivity<TSaga>
        where TSaga : class, ISaga
        where TMessage : class
    {
        readonly IStateMachineActivity<TSaga, TMessage> _activity;

        public DataConverterActivity(IStateMachineActivity<TSaga, TMessage> activity)
        {
            _activity = activity;
        }

        public void Accept(StateMachineVisitor visitor)
        {
            visitor.Visit(this, x => _activity.Accept(visitor));
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
            var dataContext = context as BehaviorContext<TSaga, TMessage>;
            if (dataContext == null)
                throw new SagaStateMachineException("Expected Type " + typeof(TMessage).Name + " but was " + context.Message.GetType().Name);

            var dataNext = next as IBehavior<TSaga, TMessage>;
            if (dataNext == null)
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
            var dataContext = context as BehaviorExceptionContext<TSaga, TMessage, TException>;
            if (dataContext == null)
                throw new SagaStateMachineException("Expected Type " + typeof(TMessage).Name + " but was " + context.Message.GetType().Name);

            var dataNext = next as IBehavior<TSaga, TMessage>;
            if (dataNext == null)
                throw new SagaStateMachineException("The next behavior was not a valid type");

            return _activity.Faulted(dataContext, dataNext);
        }
    }
}

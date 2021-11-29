namespace MassTransit.SagaStateMachine
{
    using System;
    using System.Threading.Tasks;


    public class ClearRequestActivity<TSaga, TMessage, TRequest, TResponse> :
        IStateMachineActivity<TSaga, TMessage>
        where TSaga : class, SagaStateMachineInstance
        where TRequest : class
        where TResponse : class
        where TMessage : class
    {
        readonly Request<TSaga, TRequest, TResponse> _request;

        public ClearRequestActivity(Request<TSaga, TRequest, TResponse> request)
        {
            _request = request;
        }

        public void Accept(StateMachineVisitor visitor)
        {
            visitor.Visit(this);
        }

        public Task Execute(BehaviorContext<TSaga, TMessage> context, IBehavior<TSaga, TMessage> next)
        {
            _request.SetRequestId(context.Saga, default);

            return next.Execute(context);
        }

        public Task Faulted<TException>(BehaviorExceptionContext<TSaga, TMessage, TException> context, IBehavior<TSaga, TMessage> next)
            where TException : Exception
        {
            return next.Faulted(context);
        }

        public void Probe(ProbeContext context)
        {
            context.CreateScope("clearRequest");
        }
    }
}

namespace Automatonymous.Activities
{
    using System;
    using System.Threading.Tasks;
    using GreenPipes;


    public class ClearRequestActivity<TInstance, TData, TRequest, TResponse> :
        Activity<TInstance, TData>
        where TInstance : class, SagaStateMachineInstance
        where TRequest : class
        where TResponse : class
    {
        readonly Request<TInstance, TRequest, TResponse> _request;

        public ClearRequestActivity(Request<TInstance, TRequest, TResponse> request)
        {
            _request = request;
        }

        public void Accept(StateMachineVisitor visitor)
        {
            visitor.Visit(this);
        }

        Task Activity<TInstance, TData>.Execute(BehaviorContext<TInstance, TData> context, Behavior<TInstance, TData> next)
        {
            _request.SetRequestId(context.Instance, default);

            return next.Execute(context);
        }

        public Task Faulted<TException>(BehaviorExceptionContext<TInstance, TData, TException> context, Behavior<TInstance, TData> next)
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

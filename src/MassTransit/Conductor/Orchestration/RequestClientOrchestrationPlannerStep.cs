namespace MassTransit.Conductor.Orchestration
{
    using System;
    using GreenPipes.Internals.Extensions;


    public class RequestClientOrchestrationPlannerStep<TRequest, TResponse, TResult> :
        IOrchestrationPlannerStep<TRequest, TResponse, TResult>
        where TRequest : class
        where TResponse : class
        where TResult : class
    {
        public RequestClientOrchestrationPlannerStep(Uri inputAddress)
        {
            InputAddress = inputAddress;
        }

        public Type ServiceType => typeof(TResponse);
        public Type RequestType => typeof(TRequest);
        public Uri InputAddress { get; }

        public void Build(BuildContext<TRequest, TResult> context, IOrchestrationPlan<TResponse, TResult> next)
        {
            NextBuildContext<TResponse, TResult, TRequest> nextContext = context.Create<TResponse>();
            next.Build(nextContext);

            context.RequestEndpoint(nextContext, InputAddress);
        }

        public override string ToString()
        {
            return $"Request: {TypeCache<TRequest>.ShortName} -> {TypeCache<TResponse>.ShortName}";
        }
    }
}

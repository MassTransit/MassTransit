namespace MassTransit.Conductor.Orchestration
{
    using System;


    public class NextOrchestrationPlan<TRequest, T, TResponse> :
        IOrchestrationPlan<TRequest, TResponse>
        where TRequest : class
        where TResponse : class
        where T : class
    {
        readonly IOrchestrationPlan<T, TResponse> _next;
        readonly IOrchestrationPlannerStep<TRequest, T, TResponse> _step;

        public NextOrchestrationPlan(IOrchestrationPlannerStep<TRequest, T, TResponse> step, IOrchestrationPlan<T, TResponse> next)
        {
            _step = step;
            _next = next;
        }

        public void Build(BuildContext<TRequest, TResponse> context)
        {
            _step.Build(context, _next);
        }

        public override string ToString()
        {
            return string.Join(Environment.NewLine, _step.ToString(), _next.ToString());
        }
    }
}

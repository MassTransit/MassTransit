namespace MassTransit.Conductor.Orchestration
{
    using System;
    using Context;


    public class OrchestrationProvider :
        IOrchestrationProvider
    {
        readonly IServiceDirectory _directory;

        public OrchestrationProvider(IServiceDirectory directory)
        {
            _directory = directory;
        }

        public IOrchestration<TInput, TResult> GetOrchestration<TInput, TResult>()
            where TInput : class
            where TResult : class
        {
            IOrchestrationPlanner<TResult> planner = _directory.GetOrchestrationPlanner<TResult>(typeof(TInput));

            IOrchestrationPlan<TInput, TResult> plan = planner.BuildExecutionPlan<TInput>();

            LogContext.Debug?.Log("Building Orchestration" + Environment.NewLine + "{Plan}", plan.ToString());

            var builder = new OrchestrationBuilder<TInput, TResult>();

            plan.Build(builder);

            return builder.GetExecutor();
        }
    }
}

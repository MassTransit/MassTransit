namespace MassTransit.Conductor
{
    using System.Threading.Tasks;
    using Orchestration;


    public static class OrchestrationExtensions
    {
        public static Task<TResult> Execute<TInput, TResult>(this IOrchestration<TInput, TResult> orchestration, ConsumeContext<TInput> context)
            where TResult : class
            where TInput : class
        {
            return orchestration.Execute(new ConsumeOrchestrationContext<TInput>(context, context.Message));
        }

        public static Task<TResult> Execute<TInput, TResult>(this IOrchestration<TInput, TResult> orchestration, ConsumeContext context,
            TInput request)
            where TResult : class
            where TInput : class
        {
            return orchestration.Execute(new ConsumeOrchestrationContext<TInput>(context, request));
        }
    }
}

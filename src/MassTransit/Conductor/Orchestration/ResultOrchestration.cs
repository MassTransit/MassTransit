namespace MassTransit.Conductor.Orchestration
{
    using System;
    using System.Threading.Tasks;
    using GreenPipes.Internals.Extensions;


    public class ResultOrchestration<TInput, TResult> :
        IOrchestration<TInput, TResult>
        where TInput : class
        where TResult : class
    {
        public Task<TResult> Execute(OrchestrationContext<TInput> orchestration)
        {
            var result = orchestration.Data as TResult
                ?? throw new InvalidCastException($"Result type mismatch, expected {TypeCache<TResult>.ShortName}, was {TypeCache<TInput>.ShortName}");

            return Task.FromResult(result);
        }
    }
}

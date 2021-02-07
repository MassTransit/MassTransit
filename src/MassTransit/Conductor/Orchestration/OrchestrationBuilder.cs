namespace MassTransit.Conductor.Orchestration
{
    using System;
    using GreenPipes.Internals.Extensions;


    public class OrchestrationBuilder<TInput, TResult> :
        BuildContext<TInput, TResult>
        where TInput : class
        where TResult : class
    {
        protected IOrchestration<TInput, TResult> Orchestration { get; private set; }

        public void RequestEndpoint<T>(NextBuildContext<T, TResult, TInput> context, Uri inputAddress)
            where T : class
        {
            var step = new RequestClientOrchestrationStep<TInput, T, TResult>(inputAddress);

            Orchestration = context.BuildPath(step);
        }

        public void Factory<T>(NextBuildContext<T, TResult, TInput> context, AsyncMessageFactory<TInput, T> factoryMethod)
            where T : class
        {
            var step = new FactoryOrchestrationStep<TInput, T, TResult>(factoryMethod);

            Orchestration = context.BuildPath(step);
        }

        public void Result()
        {
            Orchestration = new ResultOrchestration<TInput, TResult>();
        }

        public NextBuildContext<T, TResult, TInput> Create<T>()
            where T : class
        {
            return new OrchestrationBuilder<T, TResult, TInput>(this);
        }

        public IOrchestration<TInput, TResult> GetExecutor()
        {
            if (Orchestration == null)
                throw new InvalidOperationException("No build path configured");

            return Orchestration;
        }
    }


    public class OrchestrationBuilder<TInput, TResult, TLeft> :
        OrchestrationBuilder<TInput, TResult>,
        NextBuildContext<TInput, TResult, TLeft>
        where TResult : class
        where TInput : class
        where TLeft : class
    {
        readonly OrchestrationBuilder<TLeft, TResult> _left;

        public OrchestrationBuilder(OrchestrationBuilder<TLeft, TResult> left)
        {
            _left = left;
        }

        public IOrchestration<TLeft, TResult> BuildPath(IOrchestrationStep<TLeft, TInput, TResult> step)
        {
            if (Orchestration != null)
                return new NextStepOrchestration<TLeft, TInput, TResult>(step, Orchestration);

            if (step is IOrchestrationStep<TLeft, TResult, TResult> lastStep)
                return new LastStepOrchestration<TLeft, TResult>(lastStep);

            throw new InvalidOperationException($"Last step type mismatch, expected {TypeCache<TResult>.ShortName}, eas {TypeCache<TInput>.ShortName}");
        }
    }
}

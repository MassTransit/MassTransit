namespace MassTransit.Conductor.Orchestration
{
    using System;
    using System.Threading.Tasks;
    using Context;


    public class ConsumeOrchestrationContext<TData> :
        ConsumeContextProxy,
        OrchestrationContext<TData>
        where TData : class
    {
        ConsumeOrchestrationContext(ConsumeContext context)
            : base(context)
        {
        }

        ConsumeOrchestrationContext(OrchestrationContext left, TData value)
            : base(left)
        {
            Left = left;
            Data = value;
        }

        public ConsumeOrchestrationContext(ConsumeContext context, TData value)
            : base(context)
        {
            Left = new ConsumeOrchestrationContext<TData>(context);
            Data = value;
        }

        public Task ForEach<T>(Func<OrchestrationContext<T>, Task> callback)
            where T : class
        {
            if (this is OrchestrationContext<T> stepContext)
            {
                async Task ForEachAsync()
                {
                    await callback(stepContext).ConfigureAwait(false);

                    if (Left != null)
                        await Left.ForEach(callback).ConfigureAwait(false);
                }

                return ForEachAsync();
            }


            return Left?.ForEach(callback) ?? Task.CompletedTask;
        }

        public T Select<T>()
            where T : class
        {
            if (Data is T result)
                return result;

            return Left?.Select<T>() ?? default(T);
        }

        public TData Data { get; }

        public OrchestrationContext<T> Push<T>(T value)
            where T : class
        {
            return new ConsumeOrchestrationContext<T>(this, value);
        }

        public OrchestrationContext Left { get; }
    }
}

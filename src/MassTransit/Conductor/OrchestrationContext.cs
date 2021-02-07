namespace MassTransit.Conductor
{
    using System;
    using System.Threading.Tasks;


    public interface OrchestrationContext<out TData> :
        OrchestrationContext
    {
        TData Data { get; }

        OrchestrationContext<T> Push<T>(T value)
            where T : class;
    }


    public interface OrchestrationContext :
        ConsumeContext
    {
        OrchestrationContext Left { get; }

        public Task ForEach<T>(Func<OrchestrationContext<T>, Task> callback)
            where T : class;

        public T Select<T>()
            where T : class;
    }
}

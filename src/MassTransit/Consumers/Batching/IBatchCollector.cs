namespace MassTransit.Batching
{
    using System;
    using System.Threading.Tasks;


    public interface IBatchCollector<TMessage> :
        IAsyncDisposable,
        IProbeSite
        where TMessage : class
    {
        Task<BatchConsumer<TMessage>> Collect(ConsumeContext<TMessage> context);

        /// <summary>
        /// Complete the consumer, since it's already completed, to clear the dictionary if it matches
        /// </summary>
        /// <param name="context"></param>
        /// <param name="consumer"></param>
        Task Complete(ConsumeContext<TMessage> context, BatchConsumer<TMessage> consumer);
    }
}

namespace MassTransit.Tests.Pipeline
{
    using System.Threading.Tasks;
    using TestFramework.Messages;


    public class TwoMessageConsumer :
        IConsumer<MessageA>,
        IConsumer<MessageB>
    {
        readonly TaskCompletionSource<MessageA> _completed;
        readonly TaskCompletionSource<MessageB> _completed2;

        public TwoMessageConsumer(TaskCompletionSource<MessageA> completed, TaskCompletionSource<MessageB> completed2)
        {
            _completed = completed;
            _completed2 = completed2;
        }

        public Task<MessageA> TaskA => _completed.Task;

        public Task<MessageB> TaskB => _completed2.Task;

        async Task IConsumer<MessageA>.Consume(ConsumeContext<MessageA> context)
        {
            _completed.TrySetResult(context.Message);
        }

        async Task IConsumer<MessageB>.Consume(ConsumeContext<MessageB> context)
        {
            _completed2.TrySetResult(context.Message);
        }
    }
}

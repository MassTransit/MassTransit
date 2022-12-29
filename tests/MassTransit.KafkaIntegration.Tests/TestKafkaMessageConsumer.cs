namespace MassTransit.KafkaIntegration.Tests
{
    using System.Threading.Tasks;


    public class TestKafkaMessageConsumer<T> :
        IConsumer<T>
        where T : class
    {
        readonly TaskCompletionSource<ConsumeContext<T>> _taskCompletionSource;

        public TestKafkaMessageConsumer(TaskCompletionSource<ConsumeContext<T>> taskCompletionSource)
        {
            _taskCompletionSource = taskCompletionSource;
        }

        public async Task Consume(ConsumeContext<T> context)
        {
            _taskCompletionSource.TrySetResult(context);
        }
    }
}

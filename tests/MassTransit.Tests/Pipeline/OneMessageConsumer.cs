namespace MassTransit.Tests.Pipeline
{
    using System.Threading.Tasks;
    using TestFramework.Messages;
    using Util;


    public class OneMessageConsumer :
        IConsumer<MessageA>
    {
        readonly TaskCompletionSource<MessageA> _completed;

        public OneMessageConsumer()
        {
            _completed = TaskUtil.GetTask<MessageA>();
        }

        public OneMessageConsumer(TaskCompletionSource<MessageA> completed)
        {
            _completed = completed;
        }

        public Task<MessageA> Task
        {
            get { return _completed.Task; }
        }

        public async Task Consume(ConsumeContext<MessageA> context)
        {
            _completed.TrySetResult(context.Message);
        }
    }
}

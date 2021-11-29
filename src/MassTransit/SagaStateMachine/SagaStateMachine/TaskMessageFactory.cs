namespace MassTransit.SagaStateMachine
{
    using System;
    using System.Threading.Tasks;


    public class TaskMessageFactory<T>
        where T : class
    {
        readonly Task<SendTuple<T>> _messageFactory;

        public TaskMessageFactory(Task<SendTuple<T>> messageFactory)
        {
            _messageFactory = messageFactory;
        }

        public Task<SendTuple<T>> GetMessage()
        {
            return _messageFactory;
        }

        public Task Use(Func<SendTuple<T>, Task> callback)
        {
            Task<SendTuple<T>> msgTask = _messageFactory;
            if (msgTask.Status == TaskStatus.RanToCompletion)
                return callback(msgTask.GetAwaiter().GetResult());

            async Task GetResult()
            {
                SendTuple<T> send = await msgTask.ConfigureAwait(false);

                await callback(send).ConfigureAwait(false);
            }

            return GetResult();
        }
    }
}

namespace MassTransit.AmazonSqsTransport
{
    using System;
    using System.Threading.Tasks;


    public class BatchEntry<TEntry>
    {
        readonly TaskCompletionSource<bool> _completed;

        public BatchEntry(TEntry entry)
        {
            Entry = entry;
            _completed = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
        }

        public TEntry Entry { get; }

        public Task Completed => _completed.Task;

        public void SetCompleted()
        {
            _completed.TrySetResult(true);
        }

        public void SetFaulted(Exception exception)
        {
            _completed.TrySetException(exception);
        }
    }
}

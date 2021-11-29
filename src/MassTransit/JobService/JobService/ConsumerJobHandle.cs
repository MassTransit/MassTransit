namespace MassTransit.JobService
{
    using System;
    using System.Threading.Tasks;
    using Internals;


    public class ConsumerJobHandle<T> :
        JobHandle
        where T : class
    {
        readonly ConsumeJobContext<T> _context;

        public ConsumerJobHandle(ConsumeJobContext<T> context, Task task)
        {
            _context = context;
            JobTask = task;
        }

        public Guid JobId => _context.JobId;
        public Task JobTask { get; }

        public async Task Cancel()
        {
            if (_context.CancellationToken.IsCancellationRequested)
                return;

            _context.Cancel();

            try
            {
                await JobTask.OrTimeout(TimeSpan.FromSeconds(30)).ConfigureAwait(false);
            }
            catch (OperationCanceledException)
            {
            }
        }
    }
}

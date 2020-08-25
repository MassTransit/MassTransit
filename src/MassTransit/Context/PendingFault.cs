namespace MassTransit.Context
{
    using System;
    using System.Threading.Tasks;


    class PendingFault<T> :
        IPendingFault
        where T : class
    {
        readonly string _consumerType;
        readonly ConsumeContext<T> _context;
        readonly TimeSpan _elapsed;
        readonly Exception _exception;

        public PendingFault(ConsumeContext<T> context, TimeSpan elapsed, string consumerType, Exception exception)
        {
            _context = context;
            _elapsed = elapsed;
            _consumerType = consumerType;
            _exception = exception;
        }

        public Task Notify(ConsumeContext context)
        {
            return context.NotifyFaulted(_context, _elapsed, _consumerType, _exception);
        }
    }
}

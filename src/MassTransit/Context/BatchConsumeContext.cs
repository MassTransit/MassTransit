namespace MassTransit.Context
{
    using System;
    using System.Threading.Tasks;


    public class BatchConsumeContext<TMessage> :
        ConsumeContextScope,
        ConsumeContext<Batch<TMessage>>
        where TMessage : class
    {
        readonly ConsumeContext _context;

        public BatchConsumeContext(ConsumeContext context, Batch<TMessage> batch)
            : base(context)
        {
            _context = context;
            Message = batch;
        }

        public override Task NotifyConsumed<T>(ConsumeContext<T> context, TimeSpan duration, string consumerType)
        {
            return Task.CompletedTask;
        }

        public override Task NotifyFaulted<T>(ConsumeContext<T> context, TimeSpan duration, string consumerType, Exception exception)
        {
            return Task.CompletedTask;
        }

        public Batch<TMessage> Message { get; }

        public Task NotifyConsumed(TimeSpan duration, string consumerType)
        {
            return _context.NotifyConsumed(this, duration, consumerType);
        }

        public Task NotifyFaulted(TimeSpan duration, string consumerType, Exception exception)
        {
            return _context.NotifyFaulted(this, duration, consumerType, exception);
        }
    }
}

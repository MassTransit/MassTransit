namespace MassTransit.Transports
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;


    /// <summary>
    /// Intercepts the ISendEndpoint and makes it part of the current consume context
    /// </summary>
    public class ConsumeSendEndpoint :
        SendEndpointProxy
    {
        readonly ConsumeContext _context;
        readonly Guid? _requestId;

        public ConsumeSendEndpoint(ISendEndpoint endpoint, ConsumeContext context, Guid? requestId)
            : base(endpoint)
        {
            _context = context;
            _requestId = requestId;
        }

        public override Task Send<T>(T message, CancellationToken cancellationToken)
            where T : class
        {
            return ConsumeTask(base.Send(message, cancellationToken));
        }

        public override Task Send<T>(T message, IPipe<SendContext<T>> pipe, CancellationToken cancellationToken)
            where T : class
        {
            return ConsumeTask(base.Send(message, pipe, cancellationToken));
        }

        public override Task Send<T>(T message, IPipe<SendContext> pipe, CancellationToken cancellationToken)
            where T : class
        {
            return ConsumeTask(base.Send(message, pipe, cancellationToken));
        }

        protected override IPipe<SendContext<T>> GetPipeProxy<T>(IPipe<SendContext<T>> pipe = default)
        {
            return new ConsumeSendPipeAdapter<T>(_context, pipe, _requestId);
        }

        Task ConsumeTask(Task task)
        {
            _context.AddConsumeTask(task);
            return task;
        }
    }
}

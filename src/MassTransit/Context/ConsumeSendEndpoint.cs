namespace MassTransit.Context
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Threading;
    using System.Threading.Tasks;
    using GreenPipes;
    using Initializers;
    using Transports;


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

        protected override InitializeContext<T> GetInitializeContext<T>(IMessageInitializer<T> initializer, CancellationToken cancellationToken)
        {
            return initializer.Create(_context);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        Task ConsumeTask(Task task)
        {
            _context.AddConsumeTask(task);
            return task;
        }
    }
}

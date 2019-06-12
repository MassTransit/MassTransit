namespace MassTransit.Pipeline.Pipes
{
    using System;
    using System.Diagnostics;
    using System.Threading.Tasks;
    using GreenPipes;
    using GreenPipes.Util;


    public struct ResponsePipe :
        IPipe<SendContext>,
        IPipe<PublishContext>
    {
        readonly ConsumeContext _context;
        readonly IPipe<SendContext> _pipe;

        public ResponsePipe(ConsumeContext context, IPipe<SendContext> pipe = default)
        {
            _context = context;
            _pipe = pipe;
        }

        [DebuggerNonUserCode]
        Task IPipe<PublishContext>.Send(PublishContext context)
        {
            return Send(context);
        }

        void IProbeSite.Probe(ProbeContext context)
        {
            _pipe?.Probe(context);
        }

        public Task Send(SendContext context)
        {
            context.RequestId = _context.RequestId;
            context.SourceAddress = _context.ReceiveContext.InputAddress;

            if (_pipe.IsNotEmpty())
                return _pipe.Send(context);

            return TaskUtil.Completed;
        }
    }


    public struct ResponsePipe<T> :
        IPipe<SendContext<T>>,
        IPipe<PublishContext<T>>
        where T : class
    {
        readonly ConsumeContext _context;
        readonly IPipe<SendContext<T>> _pipe;

        public ResponsePipe(ConsumeContext context, IPipe<SendContext<T>> pipe = default)
        {
            _context = context;
            _pipe = pipe;
        }

        [DebuggerNonUserCode]
        Task IPipe<PublishContext<T>>.Send(PublishContext<T> context)
        {
            return Send(context);
        }

        void IProbeSite.Probe(ProbeContext context)
        {
            _pipe?.Probe(context);
        }

        public Task Send(SendContext<T> context)
        {
            context.RequestId = _context.RequestId;
            context.SourceAddress = _context.ReceiveContext.InputAddress;
            context.TimeToLive = _context.ExpirationTime - DateTime.UtcNow;

            if (_pipe.IsNotEmpty())
                return _pipe.Send(context);

            return TaskUtil.Completed;
        }
    }
}

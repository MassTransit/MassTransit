namespace MassTransit.AzureServiceBusTransport.Contexts
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using GreenPipes;
    using Microsoft.ServiceBus.Messaging;


    public class SharedClientContext :
        ProxyPipeContext,
        ClientContext
    {
        readonly ClientContext _context;

        public SharedClientContext(ClientContext context, CancellationToken cancellationToken)
            : base(context)
        {
            CancellationToken = cancellationToken;
            _context = context;
        }

        public override CancellationToken CancellationToken { get; }

        Uri ClientContext.InputAddress => _context.InputAddress;

        string ClientContext.EntityPath => _context.EntityPath;

        Task ClientContext.RegisterSessionHandlerFactoryAsync(IMessageSessionAsyncHandlerFactory factory,
            EventHandler<ExceptionReceivedEventArgs> exceptionHandler)
        {
            return _context.RegisterSessionHandlerFactoryAsync(factory, exceptionHandler);
        }

        void ClientContext.OnMessageAsync(Func<BrokeredMessage, Task> callback, EventHandler<ExceptionReceivedEventArgs> exceptionHandler)
        {
            _context.OnMessageAsync(callback, exceptionHandler);
        }
    }
}

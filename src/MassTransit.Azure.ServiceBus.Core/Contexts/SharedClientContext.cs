namespace MassTransit.Azure.ServiceBus.Core.Contexts
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using GreenPipes;
    using Microsoft.Azure.ServiceBus;
    using Microsoft.Azure.ServiceBus.Core;


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

        void ClientContext.OnMessageAsync(Func<IReceiverClient, Message, CancellationToken, Task> callback,
            Func<ExceptionReceivedEventArgs, Task> exceptionHandler)
        {
            _context.OnMessageAsync(callback, exceptionHandler);
        }

        void ClientContext.OnSessionAsync(Func<IMessageSession, Message, CancellationToken, Task> callback,
            Func<ExceptionReceivedEventArgs, Task> exceptionHandler)
        {
            _context.OnSessionAsync(callback, exceptionHandler);
        }

        Task ClientContext.CloseAsync(CancellationToken cancellationToken)
        {
            return _context.CloseAsync(cancellationToken);
        }
    }
}

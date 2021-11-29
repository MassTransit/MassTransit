namespace MassTransit.AzureServiceBusTransport
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Azure.Messaging.ServiceBus;
    using MassTransit.Middleware;


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

        public ConnectionContext ConnectionContext => _context.ConnectionContext;

        public Uri InputAddress => _context.InputAddress;

        public string EntityPath => _context.EntityPath;

        public bool IsClosedOrClosing => _context.IsClosedOrClosing;

        public void OnMessageAsync(Func<ProcessMessageEventArgs, ServiceBusReceivedMessage, CancellationToken, Task> callback,
            Func<ProcessErrorEventArgs, Task> exceptionHandler)
        {
            _context.OnMessageAsync(callback, exceptionHandler);
        }

        public void OnSessionAsync(Func<ProcessSessionMessageEventArgs, ServiceBusReceivedMessage, CancellationToken, Task> callback,
            Func<ProcessErrorEventArgs, Task> exceptionHandler)
        {
            _context.OnSessionAsync(callback, exceptionHandler);
        }

        public Task StartAsync()
        {
            return _context.StartAsync();
        }

        public Task ShutdownAsync()
        {
            return _context.ShutdownAsync();
        }

        public Task CloseAsync()
        {
            return _context.CloseAsync();
        }

        public Task NotifyFaulted(Exception exception, string entityPath)
        {
            return _context.NotifyFaulted(exception, entityPath);
        }
    }
}

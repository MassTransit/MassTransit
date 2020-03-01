namespace MassTransit.WebJobs.EventHubsIntegration
{
    using System;
    using System.Threading.Tasks;
    using Context;
    using Contexts;
    using GreenPipes;
    using Microsoft.Azure.EventHubs;
    using Pipeline;
    using Transports;


    public class EventDataReceiver :
        IEventDataReceiver
    {
        readonly ReceiveEndpointContext _context;
        readonly IReceivePipeDispatcher _dispatcher;

        public EventDataReceiver(ReceiveEndpointContext context)
        {
            _context = context;
            _dispatcher = new ReceivePipeDispatcher(_context.ReceivePipe, _context.ReceiveObservers, _context.LogContext);
        }

        void IProbeSite.Probe(ProbeContext context)
        {
            var scope = context.CreateScope("receiver");
            scope.Add("type", "eventData");
        }

        ConnectHandle IReceiveObserverConnector.ConnectReceiveObserver(IReceiveObserver observer)
        {
            return _context.ConnectReceiveObserver(observer);
        }

        ConnectHandle IPublishObserverConnector.ConnectPublishObserver(IPublishObserver observer)
        {
            return _context.ConnectPublishObserver(observer);
        }

        ConnectHandle ISendObserverConnector.ConnectSendObserver(ISendObserver observer)
        {
            return _context.ConnectSendObserver(observer);
        }

        async Task IEventDataReceiver.Handle(EventData message, Action<ReceiveContext> contextCallback)
        {
            var context = new EventDataReceiveContext(message, _context);
            contextCallback?.Invoke(context);

            try
            {
                await _dispatcher.Dispatch(context).ConfigureAwait(false);
            }
            finally
            {
                context.Dispose();
            }
        }

        ConnectHandle IConsumeMessageObserverConnector.ConnectConsumeMessageObserver<T>(IConsumeMessageObserver<T> observer)
        {
            return _context.ReceivePipe.ConnectConsumeMessageObserver(observer);
        }

        ConnectHandle IConsumeObserverConnector.ConnectConsumeObserver(IConsumeObserver observer)
        {
            return _context.ReceivePipe.ConnectConsumeObserver(observer);
        }
    }
}

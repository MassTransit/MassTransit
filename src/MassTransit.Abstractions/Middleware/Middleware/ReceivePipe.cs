namespace MassTransit.Middleware
{
    using System;
    using System.Threading.Tasks;
    using Transports;


    public class ReceivePipe :
        IReceivePipe
    {
        readonly IConsumePipe _consumePipe;
        readonly IPipe<ReceiveContext> _receivePipe;

        public ReceivePipe(IPipe<ReceiveContext> receivePipe, IConsumePipe consumePipe)
        {
            _receivePipe = receivePipe;
            _consumePipe = consumePipe;
        }

        public Task Connected => _consumePipe.Connected;

        Task IPipe<ReceiveContext>.Send(ReceiveContext context)
        {
            return _receivePipe.Send(context);
        }

        void IProbeSite.Probe(ProbeContext context)
        {
            _receivePipe.Probe(context);
        }

        ConnectHandle IConsumeMessageObserverConnector.ConnectConsumeMessageObserver<T>(IConsumeMessageObserver<T> observer)
        {
            return _consumePipe.ConnectConsumeMessageObserver(observer);
        }

        ConnectHandle IConsumeObserverConnector.ConnectConsumeObserver(IConsumeObserver observer)
        {
            return _consumePipe.ConnectConsumeObserver(observer);
        }

        ConnectHandle IConsumePipeConnector.ConnectConsumePipe<T>(IPipe<ConsumeContext<T>> pipe)
        {
            return _consumePipe.ConnectConsumePipe(pipe);
        }

        ConnectHandle IConsumePipeConnector.ConnectConsumePipe<T>(IPipe<ConsumeContext<T>> pipe, ConnectPipeOptions options)
        {
            return _consumePipe.ConnectConsumePipe(pipe, options);
        }

        ConnectHandle IRequestPipeConnector.ConnectRequestPipe<T>(Guid requestId, IPipe<ConsumeContext<T>> pipe)
        {
            return _consumePipe.ConnectRequestPipe(requestId, pipe);
        }
    }
}

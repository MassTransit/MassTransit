namespace MassTransit.Pipeline.Pipes
{
    using System;
    using System.Threading.Tasks;
    using GreenPipes;
    using GreenPipes.Internals.Extensions;
    using GreenPipes.Filters;
    using Util;


    public class ConsumePipe :
        IConsumePipe
    {
        readonly IDynamicFilter<ConsumeContext, Guid> _dynamicFilter;
        readonly IPipe<ConsumeContext> _pipe;
        readonly TaskCompletionSource<bool> _connected;

        public ConsumePipe(IDynamicFilter<ConsumeContext, Guid> dynamicFilter, IPipe<ConsumeContext> pipe, bool autoStart)
        {
            _dynamicFilter = dynamicFilter ?? throw new ArgumentNullException(nameof(dynamicFilter));
            _pipe = pipe ?? throw new ArgumentNullException(nameof(pipe));

            _connected = TaskUtil.GetTask<bool>();

            if (autoStart)
                _connected.TrySetResult(true);
        }

        public Task Connected => _connected.Task;

        void IProbeSite.Probe(ProbeContext context)
        {
            var scope = context.CreateScope("consumePipe");

            _pipe.Probe(scope);
        }

        Task IPipe<ConsumeContext>.Send(ConsumeContext context)
        {
            return _pipe.Send(context);
        }

        ConnectHandle IConsumeMessageObserverConnector.ConnectConsumeMessageObserver<TMessage>(IConsumeMessageObserver<TMessage> observer)
        {
            return _dynamicFilter.ConnectObserver(new ConsumeObserverAdapter<TMessage>(observer));
        }

        ConnectHandle IConsumePipeConnector.ConnectConsumePipe<T>(IPipe<ConsumeContext<T>> pipe)
        {
            var handle = _dynamicFilter.ConnectPipe(pipe);

            _connected.TrySetResultOnThreadPool(true);

            return handle;
        }

        ConnectHandle IRequestPipeConnector.ConnectRequestPipe<T>(Guid requestId, IPipe<ConsumeContext<T>> pipe)
        {
            var handle = _dynamicFilter.ConnectPipe(requestId, pipe);

            _connected.TrySetResultOnThreadPool(true);

            return handle;
        }

        ConnectHandle IConsumeObserverConnector.ConnectConsumeObserver(IConsumeObserver observer)
        {
            return _dynamicFilter.ConnectObserver(new ConsumeObserverAdapter(observer));
        }
    }
}

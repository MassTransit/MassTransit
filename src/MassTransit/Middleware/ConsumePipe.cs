namespace MassTransit.Middleware
{
    using System;
    using System.Collections.Concurrent;
    using System.Threading.Tasks;
    using Configuration;
    using Observables;
    using Transports;


    public class ConsumePipe :
        IConsumePipe
    {
        readonly TaskCompletionSource<bool> _connected;
        readonly IDynamicFilter<ConsumeContext, Guid> _dynamicFilter;
        readonly ConcurrentDictionary<Type, IMessagePipe> _outputPipes;
        readonly IPipe<ConsumeContext> _pipe;
        readonly IConsumePipeSpecification _specification;

        public ConsumePipe(IConsumePipeSpecification specification, IDynamicFilter<ConsumeContext, Guid> dynamicFilter, IPipe<ConsumeContext> pipe,
            bool autoStart)
        {
            _specification = specification;
            _dynamicFilter = dynamicFilter ?? throw new ArgumentNullException(nameof(dynamicFilter));
            _pipe = pipe ?? throw new ArgumentNullException(nameof(pipe));

            _outputPipes = new ConcurrentDictionary<Type, IMessagePipe>();
            _connected = new TaskCompletionSource<bool>(TaskCreationOptions.None | TaskCreationOptions.RunContinuationsAsynchronously);

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

        public ConnectHandle ConnectConsumePipe<T>(IPipe<ConsumeContext<T>> pipe)
            where T : class
        {
            var handle = _dynamicFilter.ConnectPipe(BuildMessagePipe(pipe));

            if (_connected.Task.Status == TaskStatus.WaitingForActivation)
                _connected.TrySetResult(true);

            return handle;
        }

        public ConnectHandle ConnectConsumePipe<T>(IPipe<ConsumeContext<T>> pipe, ConnectPipeOptions options)
            where T : class
        {
            return ConnectConsumePipe(pipe);
        }

        ConnectHandle IRequestPipeConnector.ConnectRequestPipe<T>(Guid requestId, IPipe<ConsumeContext<T>> pipe)
        {
            var handle = _dynamicFilter.ConnectPipe(requestId, BuildMessagePipe(pipe));

            if (_connected.Task.Status == TaskStatus.WaitingForActivation)
                _connected.TrySetResult(true);

            return handle;
        }

        ConnectHandle IConsumeObserverConnector.ConnectConsumeObserver(IConsumeObserver observer)
        {
            return _dynamicFilter.ConnectObserver(new ConsumeObserverAdapter(observer));
        }

        IPipe<ConsumeContext<T>> BuildMessagePipe<T>(IPipe<ConsumeContext<T>> pipe)
            where T : class
        {
            var specification = (IMessagePipe<T>)_outputPipes.GetOrAdd(typeof(T), x => new MessagePipe<T>(_specification.GetMessageSpecification<T>()));

            return specification.BuildMessagePipe(pipe);
        }


        interface IMessagePipe
        {
        }


        interface IMessagePipe<T> :
            IMessagePipe
            where T : class
        {
            IPipe<ConsumeContext<T>> BuildMessagePipe(IPipe<ConsumeContext<T>> pipe);
        }


        class MessagePipe<TMessage> :
            IMessagePipe<TMessage>
            where TMessage : class
        {
            readonly IMessageConsumePipeSpecification<TMessage> _specification;

            public MessagePipe(IMessageConsumePipeSpecification<TMessage> specification)
            {
                _specification = specification;
            }

            public IPipe<ConsumeContext<TMessage>> BuildMessagePipe(IPipe<ConsumeContext<TMessage>> pipe)
            {
                return _specification.BuildMessagePipe(pipe);
            }
        }
    }
}

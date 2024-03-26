namespace MassTransit.Middleware
{
    using System;
    using System.Collections.Concurrent;
    using System.Threading.Tasks;
    using Configuration;
    using Transports;


    public class ConsumePipe :
        IConsumePipe
    {
        readonly TaskCompletionSource<bool> _connected;
        readonly IConsumeContextMessageTypeFilter _filter;
        readonly ConcurrentDictionary<Type, IMessagePipe> _outputPipes;
        readonly IPipe<ConsumeContext> _pipe;
        readonly IConsumePipeSpecification _specification;

        public ConsumePipe(IConsumePipeSpecification specification, IConsumeContextMessageTypeFilter filter, IPipe<ConsumeContext> pipe, bool autoStart)
        {
            _specification = specification;
            _filter = filter ?? throw new ArgumentNullException(nameof(filter));
            _pipe = pipe ?? throw new ArgumentNullException(nameof(pipe));

            _outputPipes = new ConcurrentDictionary<Type, IMessagePipe>();
            _connected = new TaskCompletionSource<bool>(TaskCreationOptions.None | TaskCreationOptions.RunContinuationsAsynchronously);

            if (autoStart)
                _connected.TrySetResult(true);
        }

        public Task Connected => _connected.Task;

        public void Probe(ProbeContext context)
        {
            var scope = context.CreateScope("consumePipe");

            _pipe.Probe(scope);
        }

        public Task Send(ConsumeContext context)
        {
            return _pipe.Send(context);
        }

        public ConnectHandle ConnectConsumeMessageObserver<TMessage>(IConsumeMessageObserver<TMessage> observer)
            where TMessage : class
        {
            return _filter.ConnectConsumeMessageObserver(observer);
        }

        public ConnectHandle ConnectConsumePipe<T>(IPipe<ConsumeContext<T>> pipe)
            where T : class
        {
            var handle = _filter.ConnectMessagePipe(BuildMessagePipe(pipe));

            if (_connected.Task.Status == TaskStatus.WaitingForActivation)
                _connected.TrySetResult(true);

            return handle;
        }

        public ConnectHandle ConnectConsumePipe<T>(IPipe<ConsumeContext<T>> pipe, ConnectPipeOptions options)
            where T : class
        {
            return ConnectConsumePipe(pipe);
        }

        public ConnectHandle ConnectRequestPipe<T>(Guid requestId, IPipe<ConsumeContext<T>> pipe)
            where T : class
        {
            var handle = _filter.ConnectMessagePipe(requestId, BuildMessagePipe(pipe));

            if (_connected.Task.Status == TaskStatus.WaitingForActivation)
                _connected.TrySetResult(true);

            return handle;
        }

        public ConnectHandle ConnectConsumeObserver(IConsumeObserver observer)
        {
            return _filter.ConnectConsumeObserver(observer);
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

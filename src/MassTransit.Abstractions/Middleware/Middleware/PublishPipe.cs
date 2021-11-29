namespace MassTransit.Middleware
{
    using System;
    using System.Collections.Concurrent;
    using System.Diagnostics;
    using System.Threading.Tasks;
    using Configuration;
    using Transports;


    public class PublishPipe :
        IPublishPipe
    {
        readonly ConcurrentDictionary<Type, IMessagePipe> _outputPipes;
        readonly IPublishPipeSpecification _specification;

        public PublishPipe(IPublishPipeSpecification specification)
        {
            _specification = specification;
            _outputPipes = new ConcurrentDictionary<Type, IMessagePipe>();
        }

        public void Probe(ProbeContext context)
        {
            var scope = context.CreateScope("publishPipe");

            foreach (var outputPipe in _outputPipes.Values)
                outputPipe.Probe(scope);
        }

        [DebuggerNonUserCode]
        [DebuggerStepThrough]
        public Task Send<T>(PublishContext<T> context)
            where T : class
        {
            return _outputPipes.GetOrAdd(typeof(T), x => new MessagePipe<T>(_specification.GetMessageSpecification<T>())).Send(context);
        }


        interface IMessagePipe :
            IProbeSite
        {
            Task Send<T>(PublishContext<T> context)
                where T : class;
        }


        class MessagePipe<TMessage> :
            IMessagePipe
            where TMessage : class
        {
            readonly Lazy<IMessagePublishPipe<TMessage>> _output;
            readonly IMessagePublishPipeSpecification<TMessage> _specification;

            public MessagePipe(IMessagePublishPipeSpecification<TMessage> specification)
            {
                _output = new Lazy<IMessagePublishPipe<TMessage>>(CreateMessagePipe);

                _specification = specification;
            }

            public Task Send<T>(PublishContext<T> context)
                where T : class
            {
                return _output.Value.Send((PublishContext<TMessage>)context);
            }

            public void Probe(ProbeContext context)
            {
                _output.Value.Probe(context);
            }

            IMessagePublishPipe<TMessage> CreateMessagePipe()
            {
                IPipe<PublishContext<TMessage>> messagePipe = _specification.BuildMessagePipe();

                return new MessagePublishPipe<TMessage>(messagePipe);
            }
        }
    }
}

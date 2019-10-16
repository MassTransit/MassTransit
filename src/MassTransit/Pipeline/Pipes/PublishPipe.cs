namespace MassTransit.Pipeline.Pipes
{
    using System;
    using System.Collections.Concurrent;
    using System.Diagnostics;
    using System.Threading.Tasks;
    using GreenPipes;
    using PublishPipeSpecifications;


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

        void IProbeSite.Probe(ProbeContext context)
        {
            var scope = context.CreateScope("publishPipe");

            foreach (var outputPipe in _outputPipes.Values)
            {
                outputPipe.Probe(scope);
            }
        }

        [DebuggerNonUserCode]
        [DebuggerStepThrough]
        Task IPublishPipe.Send<T>(PublishContext<T> context)
        {
            return GetPipe<T>().Send(context);
        }

        IMessagePipe GetPipe<T>()
            where T : class
        {
            return _outputPipes.GetOrAdd(typeof(T), x => new MessagePipe<T>(_specification.GetMessageSpecification<T>()));
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
                _output = new Lazy<IMessagePublishPipe<TMessage>>(CreateFilter);

                _specification = specification;
            }

            Task IMessagePipe.Send<T>(PublishContext<T> context)
            {
                return _output.Value.Send((PublishContext<TMessage>)context);
            }

            public void Probe(ProbeContext context)
            {
                _output.Value.Probe(context);
            }

            IMessagePublishPipe<TMessage> CreateFilter()
            {
                IPipe<PublishContext<TMessage>> messagePipe = _specification.BuildMessagePipe();

                return new MessagePublishPipe<TMessage>(messagePipe);
            }
        }
    }
}

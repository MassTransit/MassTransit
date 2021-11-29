namespace MassTransit.Configuration
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using Internals;
    using Metadata;


    public class PublishPipeSpecification :
        IPublishPipeConfigurator,
        IPublishPipeSpecification
    {
        readonly object _lock = new object();
        readonly ConcurrentDictionary<Type, IMessagePublishPipeSpecification> _messageSpecifications;
        readonly PublishPipeSpecificationObservable _observers;
        readonly IList<IPipeSpecification<PublishContext>> _specifications;

        public PublishPipeSpecification()
        {
            _specifications = new List<IPipeSpecification<PublishContext>>();
            _messageSpecifications = new ConcurrentDictionary<Type, IMessagePublishPipeSpecification>();
            _observers = new PublishPipeSpecificationObservable();
        }

        public void AddPipeSpecification(IPipeSpecification<PublishContext> specification)
        {
            lock (_lock)
            {
                _specifications.Add(specification);

                foreach (var messageSpecification in _messageSpecifications.Values)
                    messageSpecification.AddPipeSpecification(specification);
            }
        }

        public void AddPipeSpecification<T>(IPipeSpecification<PublishContext<T>> specification)
            where T : class
        {
            IMessagePublishPipeSpecification<T> messageSpecification = GetMessageSpecification<T>();

            messageSpecification.AddPipeSpecification(specification);
        }

        void IPublishPipeConfigurator.AddPipeSpecification(IPipeSpecification<SendContext> specification)
        {
            var splitSpecification = new SplitFilterPipeSpecification<PublishContext, SendContext>(specification, MergeContext, FilterContext);

            AddPipeSpecification(splitSpecification);
        }

        void IPublishPipeConfigurator.AddPipeSpecification<T>(IPipeSpecification<SendContext<T>> specification)
        {
            var splitSpecification = new SplitFilterPipeSpecification<PublishContext<T>, SendContext<T>>(specification, MergeContext, FilterContext);

            AddPipeSpecification(splitSpecification);
        }

        public ConnectHandle ConnectPublishPipeSpecificationObserver(IPublishPipeSpecificationObserver observer)
        {
            return _observers.Connect(observer);
        }

        public IEnumerable<ValidationResult> Validate()
        {
            lock (_lock)
            {
                return _specifications.SelectMany(x => x.Validate())
                    .Concat(_messageSpecifications.Values.SelectMany(x => x.Validate()));
            }
        }

        public IMessagePublishPipeSpecification<T> GetMessageSpecification<T>()
            where T : class
        {
            var specification = _messageSpecifications.GetOrAdd(typeof(T), CreateMessageSpecification<T>);

            return specification.GetMessageSpecification<T>();
        }

        static SendContext<T> FilterContext<T>(PublishContext<T> context)
            where T : class
        {
            return context;
        }

        static PublishContext<T> MergeContext<T>(PublishContext<T> input, SendContext context)
            where T : class
        {
            return context.GetPayload<PublishContext<T>>();
        }

        static SendContext FilterContext(PublishContext context)
        {
            return context;
        }

        static PublishContext MergeContext(PublishContext input, SendContext context)
        {
            return context.GetPayload<PublishContext>();
        }

        IMessagePublishPipeSpecification CreateMessageSpecification<T>(Type type)
            where T : class
        {
            var specification = new MessagePublishPipeSpecification<T>();

            lock (_lock)
            {
                foreach (IPipeSpecification<PublishContext> pipeSpecification in _specifications)
                    specification.AddPipeSpecification(pipeSpecification);
            }

            _observers.MessageSpecificationCreated(specification);

            var connector = new ImplementedMessageTypeConnector<T>(this, specification);

            ImplementedMessageTypeCache<T>.EnumerateImplementedTypes(connector);

            return specification;
        }


        class ImplementedMessageTypeConnector<TMessage> :
            IImplementedMessageType
            where TMessage : class
        {
            readonly MessagePublishPipeSpecification<TMessage> _messageSpecification;
            readonly IPublishPipeSpecification _specification;

            public ImplementedMessageTypeConnector(IPublishPipeSpecification specification, MessagePublishPipeSpecification<TMessage> messageSpecification)
            {
                _specification = specification;
                _messageSpecification = messageSpecification;
            }

            public void ImplementsMessageType<T>(bool direct)
                where T : class
            {
                // Do not create implemented types for message types that have been excluded
                if (typeof(T).HasAttribute<ExcludeFromImplementedTypesAttribute>())
                    return;

                IMessagePublishPipeSpecification<T> implementedTypeSpecification = _specification.GetMessageSpecification<T>();

                _messageSpecification.AddImplementedMessageSpecification(implementedTypeSpecification);
            }
        }
    }
}

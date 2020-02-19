namespace MassTransit.PublishPipeSpecifications
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using GreenPipes;
    using GreenPipes.Specifications;
    using GreenPipes.Util;
    using Metadata;
    using PipeBuilders;


    public class MessagePublishPipeSpecification<TMessage> :
        IMessagePublishPipeSpecification<TMessage>,
        IMessagePublishPipeSpecification
        where TMessage : class
    {
        readonly IList<ISpecificationPipeSpecification<PublishContext<TMessage>>> _implementedMessageTypeSpecifications;
        readonly IList<ISpecificationPipeSpecification<PublishContext<TMessage>>> _parentMessageSpecifications;
        readonly IList<IPipeSpecification<PublishContext<TMessage>>> _specifications;
        readonly IList<IPipeSpecification<PublishContext>> _baseSpecifications;

        public MessagePublishPipeSpecification()
        {
            _specifications = new List<IPipeSpecification<PublishContext<TMessage>>>();
            _baseSpecifications = new List<IPipeSpecification<PublishContext>>();
            _implementedMessageTypeSpecifications = new List<ISpecificationPipeSpecification<PublishContext<TMessage>>>();
            _parentMessageSpecifications = new List<ISpecificationPipeSpecification<PublishContext<TMessage>>>();
        }

        public void AddPipeSpecification(IPipeSpecification<PublishContext> specification)
        {
            _baseSpecifications.Add(specification);
        }

        IMessagePublishPipeSpecification<T> IMessagePublishPipeSpecification.GetMessageSpecification<T>()
        {
            var result = this as IMessagePublishPipeSpecification<T>;
            if (result == null)
                throw new ArgumentException($"The expected message type was invalid: {TypeMetadataCache<T>.ShortName}");

            return result;
        }

        public ConnectHandle Connect(IPipeConnector connector)
        {
            IPipe<PublishContext<TMessage>> messagePipe = BuildMessagePipe();

            return messagePipe.IsNotEmpty()
                ? connector.ConnectPipe(messagePipe)
                : new EmptyConnectHandle();
        }

        public void AddPipeSpecification(IPipeSpecification<PublishContext<TMessage>> specification)
        {
            _specifications.Add(specification);
        }

        public IEnumerable<ValidationResult> Validate()
        {
            return _specifications.SelectMany(x => x.Validate()).Concat(_baseSpecifications.SelectMany(x => x.Validate()));
        }

        public void Apply(ISpecificationPipeBuilder<PublishContext<TMessage>> builder)
        {
            if (!builder.IsDelegated && _implementedMessageTypeSpecifications.Count > 0)
            {
                ISpecificationPipeBuilder<PublishContext<TMessage>> implementedBuilder = builder.CreateImplementedBuilder();

                foreach (ISpecificationPipeSpecification<PublishContext<TMessage>> specification in _implementedMessageTypeSpecifications.Reverse())
                {
                    specification.Apply(implementedBuilder);
                }
            }

            ISpecificationPipeBuilder<PublishContext<TMessage>> delegatedBuilder = builder.CreateDelegatedBuilder();

            foreach (ISpecificationPipeSpecification<PublishContext<TMessage>> specification in _parentMessageSpecifications)
            {
                specification.Apply(delegatedBuilder);
            }

            foreach (IPipeSpecification<PublishContext<TMessage>> specification in _specifications)
            {
                specification.Apply(builder);
            }

            if (!builder.IsImplemented)
            {
                foreach (IPipeSpecification<PublishContext> specification in _baseSpecifications)
                {
                    var split = new SplitFilterPipeSpecification<PublishContext<TMessage>, PublishContext>(specification, MergeContext, FilterContext);

                    split.Apply(builder);
                }
            }
        }

        public IPipe<PublishContext<TMessage>> BuildMessagePipe()
        {
            var pipeBuilder = new SpecificationPipeBuilder<PublishContext<TMessage>>();

            Apply(pipeBuilder);

            return pipeBuilder.Build();
        }

        public void AddParentMessageSpecification(ISpecificationPipeSpecification<PublishContext<TMessage>> implementedMessageTypeSpecification)
        {
            _parentMessageSpecifications.Add(implementedMessageTypeSpecification);
        }

        public void AddImplementedMessageSpecification<T>(ISpecificationPipeSpecification<PublishContext<T>> implementedMessageTypeSpecification)
            where T : class
        {
            var adapter = new ImplementedTypeAdapter<T>(implementedMessageTypeSpecification);

            _implementedMessageTypeSpecifications.Add(adapter);
        }

        static PublishContext FilterContext(PublishContext<TMessage> context)
        {
            return context;
        }

        static PublishContext<TMessage> MergeContext(PublishContext<TMessage> input, PublishContext context)
        {
            return context.GetPayload<PublishContext<TMessage>>();
        }


        class ImplementedTypeAdapter<T> :
            ISpecificationPipeSpecification<PublishContext<TMessage>>
            where T : class
        {
            readonly ISpecificationPipeSpecification<PublishContext<T>> _specification;

            public ImplementedTypeAdapter(ISpecificationPipeSpecification<PublishContext<T>> specification)
            {
                _specification = specification;
            }

            public void Apply(ISpecificationPipeBuilder<PublishContext<TMessage>> builder)
            {
                var specification = new MessagePublishPipeSplitFilterSpecification<TMessage, T>(_specification);

                specification.Apply(builder);
            }

            public IEnumerable<ValidationResult> Validate()
            {
                yield break;
            }
        }
    }
}

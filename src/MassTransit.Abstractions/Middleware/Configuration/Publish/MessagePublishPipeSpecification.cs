namespace MassTransit.Configuration
{
    using System;
    using System.Collections.Generic;
    using System.Linq;


    public class MessagePublishPipeSpecification<TMessage> :
        IMessagePublishPipeSpecification<TMessage>,
        IMessagePublishPipeSpecification
        where TMessage : class
    {
        readonly IList<IPipeSpecification<PublishContext>> _baseSpecifications;
        readonly IList<ISpecificationPipeSpecification<PublishContext<TMessage>>> _implementedMessageTypeSpecifications;
        readonly IList<ISpecificationPipeSpecification<PublishContext<TMessage>>> _parentMessageSpecifications;
        readonly IList<IPipeSpecification<PublishContext<TMessage>>> _specifications;

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
            if (this is IMessagePublishPipeSpecification<T> result)
                return result;

            throw new ArgumentException($"The expected message type was invalid: {TypeCache<T>.ShortName}");
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

                for (var index = _implementedMessageTypeSpecifications.Count - 1; index >= 0; index--)
                    _implementedMessageTypeSpecifications[index].Apply(implementedBuilder);
            }

            var parentCount = _parentMessageSpecifications.Count;
            if (parentCount > 0)
            {
                ISpecificationPipeBuilder<PublishContext<TMessage>> delegatedBuilder = builder.CreateDelegatedBuilder();

                for (var index = 0; index < parentCount; index++)
                    _parentMessageSpecifications[index].Apply(delegatedBuilder);
            }

            for (var index = 0; index < _specifications.Count; index++)
                _specifications[index].Apply(builder);

            if (!builder.IsImplemented)
            {
                for (var index = 0; index < _baseSpecifications.Count; index++)
                {
                    var split = new SplitFilterPipeSpecification<PublishContext<TMessage>, PublishContext>(_baseSpecifications[index], MergeContext,
                        FilterContext);

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

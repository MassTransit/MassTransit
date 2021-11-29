namespace MassTransit.Configuration
{
    using System;
    using System.Collections.Generic;
    using System.Linq;


    public class MessageSendPipeSpecification<TMessage> :
        IMessageSendPipeSpecification<TMessage>,
        IMessageSendPipeSpecification
        where TMessage : class
    {
        readonly IList<IPipeSpecification<SendContext>> _baseSpecifications;
        readonly IList<ISpecificationPipeSpecification<SendContext<TMessage>>> _implementedMessageTypeSpecifications;
        readonly IList<ISpecificationPipeSpecification<SendContext<TMessage>>> _parentMessageSpecifications;
        readonly IList<IPipeSpecification<SendContext<TMessage>>> _specifications;

        public MessageSendPipeSpecification()
        {
            _specifications = new List<IPipeSpecification<SendContext<TMessage>>>();
            _baseSpecifications = new List<IPipeSpecification<SendContext>>();
            _implementedMessageTypeSpecifications = new List<ISpecificationPipeSpecification<SendContext<TMessage>>>();
            _parentMessageSpecifications = new List<ISpecificationPipeSpecification<SendContext<TMessage>>>();
        }

        public void AddPipeSpecification(IPipeSpecification<SendContext> specification)
        {
            _baseSpecifications.Add(specification);
        }

        IMessageSendPipeSpecification<T> IMessageSendPipeSpecification.GetMessageSpecification<T>()
        {
            if (this is IMessageSendPipeSpecification<T> result)
                return result;

            throw new ArgumentException($"The expected message type was invalid: {TypeCache<T>.ShortName}");
        }

        public void AddPipeSpecification(IPipeSpecification<SendContext<TMessage>> specification)
        {
            _specifications.Add(specification);
        }

        public IEnumerable<ValidationResult> Validate()
        {
            return _specifications.SelectMany(x => x.Validate());
        }

        public void Apply(ISpecificationPipeBuilder<SendContext<TMessage>> builder)
        {
            if (!builder.IsDelegated && _implementedMessageTypeSpecifications.Count > 0)
            {
                ISpecificationPipeBuilder<SendContext<TMessage>> implementedBuilder = builder.CreateImplementedBuilder();

                for (var index = _implementedMessageTypeSpecifications.Count - 1; index >= 0; index--)
                    _implementedMessageTypeSpecifications[index].Apply(implementedBuilder);
            }

            var parentCount = _parentMessageSpecifications.Count;
            if (parentCount > 0)
            {
                ISpecificationPipeBuilder<SendContext<TMessage>> delegatedBuilder = builder.CreateDelegatedBuilder();

                for (var index = 0; index < parentCount; index++)
                    _parentMessageSpecifications[index].Apply(delegatedBuilder);
            }

            for (var index = 0; index < _specifications.Count; index++)
                _specifications[index].Apply(builder);

            if (!builder.IsImplemented)
            {
                for (var index = 0; index < _baseSpecifications.Count; index++)
                {
                    var split = new SplitFilterPipeSpecification<SendContext<TMessage>, SendContext>(_baseSpecifications[index], MergeContext, FilterContext);

                    split.Apply(builder);
                }
            }
        }

        public IPipe<SendContext<TMessage>> BuildMessagePipe()
        {
            var pipeBuilder = new SpecificationPipeBuilder<SendContext<TMessage>>();

            Apply(pipeBuilder);

            return pipeBuilder.Build();
        }

        public void AddParentMessageSpecification(ISpecificationPipeSpecification<SendContext<TMessage>> parentSpecification)
        {
            _parentMessageSpecifications.Add(parentSpecification);
        }

        public void AddImplementedMessageSpecification<T>(ISpecificationPipeSpecification<SendContext<T>> implementedSpecification)
            where T : class
        {
            var adapter = new ImplementedTypeAdapter<T>(implementedSpecification);

            _implementedMessageTypeSpecifications.Add(adapter);
        }

        static SendContext FilterContext(SendContext<TMessage> context)
        {
            return context;
        }

        static SendContext<TMessage> MergeContext(SendContext<TMessage> input, SendContext context)
        {
            return context.GetPayload<SendContext<TMessage>>();
        }


        class ImplementedTypeAdapter<T> :
            ISpecificationPipeSpecification<SendContext<TMessage>>
            where T : class
        {
            readonly ISpecificationPipeSpecification<SendContext<T>> _specification;

            public ImplementedTypeAdapter(ISpecificationPipeSpecification<SendContext<T>> specification)
            {
                _specification = specification;
            }

            public void Apply(ISpecificationPipeBuilder<SendContext<TMessage>> builder)
            {
                var specification = new MessageSendPipeSplitFilterSpecification<TMessage, T>(_specification);

                specification.Apply(builder);
            }

            public IEnumerable<ValidationResult> Validate()
            {
                yield break;
            }
        }
    }
}

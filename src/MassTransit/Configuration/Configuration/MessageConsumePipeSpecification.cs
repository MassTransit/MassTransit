namespace MassTransit.Configuration
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Context;


    public class MessageConsumePipeSpecification<TMessage> :
        IMessageConsumePipeSpecification<TMessage>,
        IMessageConsumePipeSpecification
        where TMessage : class
    {
        readonly IList<IPipeSpecification<ConsumeContext>> _baseSpecifications;
        readonly IList<ISpecificationPipeSpecification<ConsumeContext<TMessage>>> _parentMessageSpecifications;
        readonly IList<IPipeSpecification<ConsumeContext<TMessage>>> _specifications;

        public MessageConsumePipeSpecification()
        {
            _specifications = new List<IPipeSpecification<ConsumeContext<TMessage>>>();
            _baseSpecifications = new List<IPipeSpecification<ConsumeContext>>();
            _parentMessageSpecifications = new List<ISpecificationPipeSpecification<ConsumeContext<TMessage>>>();
        }

        public void AddPipeSpecification(IPipeSpecification<ConsumeContext> specification)
        {
            _baseSpecifications.Add(specification);
        }

        IMessageConsumePipeSpecification<T> IMessageConsumePipeSpecification.GetMessageSpecification<T>()
        {
            if (this is IMessageConsumePipeSpecification<T> result)
                return result;

            throw new ArgumentException($"The expected message type was invalid: {TypeCache<T>.ShortName}");
        }

        public void AddPipeSpecification(IPipeSpecification<ConsumeContext<TMessage>> specification)
        {
            _specifications.Add(specification);
        }

        public IEnumerable<ValidationResult> Validate()
        {
            return _specifications.SelectMany(x => x.Validate());
        }

        public void Apply(ISpecificationPipeBuilder<ConsumeContext<TMessage>> builder)
        {
            var parentCount = _parentMessageSpecifications.Count;
            if (parentCount > 0)
            {
                ISpecificationPipeBuilder<ConsumeContext<TMessage>> delegatedBuilder = builder.CreateDelegatedBuilder();

                for (var index = 0; index < parentCount; index++)
                    _parentMessageSpecifications[index].Apply(delegatedBuilder);
            }

            for (var index = 0; index < _specifications.Count; index++)
                _specifications[index].Apply(builder);

            if (!builder.IsImplemented)
            {
                for (var index = 0; index < _baseSpecifications.Count; index++)
                {
                    var split = new SplitFilterPipeSpecification<ConsumeContext<TMessage>, ConsumeContext>(_baseSpecifications[index], MergeContext,
                        FilterContext);
                    split.Apply(builder);
                }
            }
        }

        public IPipe<ConsumeContext<TMessage>> BuildMessagePipe(IPipe<ConsumeContext<TMessage>> pipe)
        {
            var pipeBuilder = new SpecificationPipeBuilder<ConsumeContext<TMessage>>();

            Apply(pipeBuilder);

            return pipeBuilder.Build(pipe);
        }

        public void AddParentMessageSpecification(ISpecificationPipeSpecification<ConsumeContext<TMessage>> parentSpecification)
        {
            _parentMessageSpecifications.Add(parentSpecification);
        }

        static ConsumeContext FilterContext(ConsumeContext<TMessage> context)
        {
            return context;
        }

        static ConsumeContext<TMessage> MergeContext(ConsumeContext<TMessage> input, ConsumeContext context)
        {
            var result = context as ConsumeContext<TMessage>;

            return result ?? new MessageConsumeContext<TMessage>(context, input.Message);
        }
    }
}

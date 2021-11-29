namespace MassTransit.Configuration
{
    using System.Collections.Generic;


    public class MessagePublishTopologyPipeSpecification<TMessage> :
        ISpecificationPipeSpecification<PublishContext<TMessage>>
        where TMessage : class
    {
        readonly IMessagePublishTopology<TMessage> _messagePublishTopology;

        public MessagePublishTopologyPipeSpecification(IMessagePublishTopology<TMessage> messagePublishTopology)
        {
            _messagePublishTopology = messagePublishTopology;
        }

        public void Apply(ISpecificationPipeBuilder<PublishContext<TMessage>> builder)
        {
            var typeBuilder = new Builder(builder);

            _messagePublishTopology.Apply(typeBuilder);
        }

        public IEnumerable<ValidationResult> Validate()
        {
            yield break;
        }


        class Builder :
            ITopologyPipeBuilder<PublishContext<TMessage>>
        {
            readonly ISpecificationPipeBuilder<PublishContext<TMessage>> _builder;

            public Builder(ISpecificationPipeBuilder<PublishContext<TMessage>> builder)
            {
                _builder = builder;
            }

            public void AddFilter(IFilter<PublishContext<TMessage>> filter)
            {
                _builder.AddFilter(filter);
            }

            public bool IsDelegated => _builder.IsDelegated;
            public bool IsImplemented => _builder.IsImplemented;

            public ITopologyPipeBuilder<PublishContext<TMessage>> CreateDelegatedBuilder()
            {
                return new ChildBuilder<PublishContext<TMessage>>(this, IsImplemented, true);
            }


            class ChildBuilder<T> :
                ITopologyPipeBuilder<T>
                where T : class, PipeContext
            {
                readonly ITopologyPipeBuilder<T> _builder;

                public ChildBuilder(ITopologyPipeBuilder<T> builder, bool isImplemented, bool isDelegated)
                {
                    _builder = builder;

                    IsDelegated = isDelegated;
                    IsImplemented = isImplemented;
                }

                public void AddFilter(IFilter<T> filter)
                {
                    _builder.AddFilter(filter);
                }

                public bool IsDelegated { get; }

                public bool IsImplemented { get; }

                public ITopologyPipeBuilder<T> CreateDelegatedBuilder()
                {
                    return new ChildBuilder<T>(this, IsImplemented, true);
                }
            }
        }
    }
}

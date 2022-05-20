namespace MassTransit.Configuration
{
    using System.Collections.Generic;
    using Middleware;


    public class MessagePublishPipeSplitFilterSpecification<TMessage, T> :
        ISpecificationPipeSpecification<PublishContext<TMessage>>
        where TMessage : class
        where T : class
    {
        readonly ISpecificationPipeSpecification<PublishContext<T>> _specification;

        public MessagePublishPipeSplitFilterSpecification(ISpecificationPipeSpecification<PublishContext<T>> specification)
        {
            _specification = specification;
        }

        public void Apply(ISpecificationPipeBuilder<PublishContext<TMessage>> builder)
        {
            var splitBuilder = new Builder(builder);

            _specification.Apply(splitBuilder);
        }

        public IEnumerable<ValidationResult> Validate()
        {
            yield break;
        }


        class Builder :
            ISpecificationPipeBuilder<PublishContext<T>>
        {
            readonly ISpecificationPipeBuilder<PublishContext<TMessage>> _builder;

            public Builder(ISpecificationPipeBuilder<PublishContext<TMessage>> builder)
            {
                _builder = builder;
            }

            public void AddFilter(IFilter<PublishContext<T>> filter)
            {
                var splitFilter = new SplitFilter<PublishContext<TMessage>, PublishContext<T>>(filter, ContextProvider, InputContextProvider);

                _builder.AddFilter(splitFilter);
            }

            public bool IsDelegated => _builder.IsDelegated;
            public bool IsImplemented => _builder.IsImplemented;

            public ISpecificationPipeBuilder<PublishContext<T>> CreateDelegatedBuilder()
            {
                return new ChildSpecificationPipeBuilder<PublishContext<T>>(this, IsImplemented, true);
            }

            public ISpecificationPipeBuilder<PublishContext<T>> CreateImplementedBuilder()
            {
                return new ChildSpecificationPipeBuilder<PublishContext<T>>(this, true, IsDelegated);
            }

            PublishContext<TMessage> ContextProvider(PublishContext<TMessage> context, PublishContext<T> splitContext)
            {
                return context;
            }

            static PublishContext<T> InputContextProvider(PublishContext<TMessage> context)
            {
                return (PublishContext<T>)context;
            }
        }
    }
}

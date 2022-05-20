namespace MassTransit.Configuration
{
    using System.Collections.Generic;
    using Middleware;


    public class MessageSendPipeSplitFilterSpecification<TMessage, T> :
        ISpecificationPipeSpecification<SendContext<TMessage>>
        where TMessage : class
        where T : class
    {
        readonly ISpecificationPipeSpecification<SendContext<T>> _specification;

        public MessageSendPipeSplitFilterSpecification(ISpecificationPipeSpecification<SendContext<T>> specification)
        {
            _specification = specification;
        }

        public void Apply(ISpecificationPipeBuilder<SendContext<TMessage>> builder)
        {
            var splitBuilder = new Builder(builder);

            _specification.Apply(splitBuilder);
        }

        public IEnumerable<ValidationResult> Validate()
        {
            yield break;
        }


        class Builder :
            ISpecificationPipeBuilder<SendContext<T>>
        {
            readonly ISpecificationPipeBuilder<SendContext<TMessage>> _builder;

            public Builder(ISpecificationPipeBuilder<SendContext<TMessage>> builder)
            {
                _builder = builder;
            }

            public void AddFilter(IFilter<SendContext<T>> filter)
            {
                var splitFilter = new SplitFilter<SendContext<TMessage>, SendContext<T>>(filter, ContextProvider, InputContextProvider);

                _builder.AddFilter(splitFilter);
            }

            public bool IsDelegated => _builder.IsDelegated;
            public bool IsImplemented => _builder.IsImplemented;

            public ISpecificationPipeBuilder<SendContext<T>> CreateDelegatedBuilder()
            {
                return new ChildSpecificationPipeBuilder<SendContext<T>>(this, IsImplemented, true);
            }

            public ISpecificationPipeBuilder<SendContext<T>> CreateImplementedBuilder()
            {
                return new ChildSpecificationPipeBuilder<SendContext<T>>(this, true, IsDelegated);
            }

            SendContext<TMessage> ContextProvider(SendContext<TMessage> context, SendContext<T> splitContext)
            {
                return context;
            }

            static SendContext<T> InputContextProvider(SendContext<TMessage> context)
            {
                return (SendContext<T>)context;
            }
        }
    }
}

namespace MassTransit.PipeConfigurators
{
    using System.Collections.Generic;
    using GreenPipes;
    using Pipeline.Filters;
    using Scoping;


    public class ScopeSendPipeSpecification<T> :
        IPipeSpecification<SendContext<T>>
        where T : class
    {
        readonly ISendScopeProvider _sendScopeProvider;

        public ScopeSendPipeSpecification(ISendScopeProvider sendScopeProvider)
        {
            _sendScopeProvider = sendScopeProvider;
        }
        public void Apply(IPipeBuilder<SendContext<T>> builder)
        {
            builder.AddFilter(new ScopeSendFilter<T>(_sendScopeProvider));
        }

        public IEnumerable<ValidationResult> Validate()
        {
            if (_sendScopeProvider == null)
                yield return this.Failure(nameof(_sendScopeProvider), "Should not be null.");
        }
    }
}

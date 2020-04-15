namespace MassTransit.PipeConfigurators
{
    using System.Collections.Generic;
    using GreenPipes;
    using Pipeline.Filters;
    using Scoping;


    public class ScopePublishPipeSpecification<T> :
        IPipeSpecification<PublishContext<T>>
        where T : class
    {
        readonly IPublishScopeProvider _sendScopeProvider;

        public ScopePublishPipeSpecification(IPublishScopeProvider sendScopeProvider)
        {
            _sendScopeProvider = sendScopeProvider;
        }
        public void Apply(IPipeBuilder<PublishContext<T>> builder)
        {
            builder.AddFilter(new ScopePublishFilter<T>(_sendScopeProvider));
        }

        public IEnumerable<ValidationResult> Validate()
        {
            if (_sendScopeProvider == null)
                yield return this.Failure(nameof(_sendScopeProvider), "Should not be null.");
        }
    }
}

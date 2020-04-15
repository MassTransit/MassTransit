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
        readonly IPublishScopeProvider _publishScopeProvider;

        public ScopePublishPipeSpecification(IPublishScopeProvider publishScopeProvider)
        {
            _publishScopeProvider = publishScopeProvider;
        }
        public void Apply(IPipeBuilder<PublishContext<T>> builder)
        {
            builder.AddFilter(new ScopePublishFilter<T>(_publishScopeProvider));
        }

        public IEnumerable<ValidationResult> Validate()
        {
            if (_publishScopeProvider == null)
                yield return this.Failure(nameof(_publishScopeProvider), "Should not be null.");
        }
    }
}

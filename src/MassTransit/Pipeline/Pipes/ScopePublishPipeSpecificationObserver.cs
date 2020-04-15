namespace MassTransit.Pipeline.Pipes
{
    using PipeConfigurators;
    using PublishPipeSpecifications;
    using Scoping;


    public class ScopePublishPipeSpecificationObserver :
        IPublishPipeSpecificationObserver
    {
        readonly IPublishScopeProvider _publishScopeProvider;

        public ScopePublishPipeSpecificationObserver(IPublishScopeProvider publishScopeProvider)
        {
            _publishScopeProvider = publishScopeProvider;
        }
        public void MessageSpecificationCreated<T>(IMessagePublishPipeSpecification<T> specification)
            where T : class
        {
            specification.AddPipeSpecification(new ScopePublishPipeSpecification<T>(_publishScopeProvider));
        }
    }
}

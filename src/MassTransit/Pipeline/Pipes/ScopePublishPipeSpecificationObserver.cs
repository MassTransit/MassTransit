namespace MassTransit.Pipeline.Pipes
{
    using PipeConfigurators;
    using PublishPipeSpecifications;
    using Scoping;


    public class ScopePublishPipeSpecificationObserver :
        IPublishPipeSpecificationObserver
    {
        readonly IPublishScopeProvider _sendScopeProvider;

        public ScopePublishPipeSpecificationObserver(IPublishScopeProvider sendScopeProvider)
        {
            _sendScopeProvider = sendScopeProvider;
        }
        public void MessageSpecificationCreated<T>(IMessagePublishPipeSpecification<T> specification)
            where T : class
        {
            specification.AddPipeSpecification(new ScopePublishPipeSpecification<T>(_sendScopeProvider));
        }
    }
}

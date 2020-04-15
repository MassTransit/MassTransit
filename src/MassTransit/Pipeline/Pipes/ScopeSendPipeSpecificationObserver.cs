namespace MassTransit.Pipeline.Pipes
{
    using PipeConfigurators;
    using Scoping;
    using SendPipeSpecifications;


    public class ScopeSendPipeSpecificationObserver :
        ISendPipeSpecificationObserver
    {
        readonly ISendScopeProvider _sendScopeProvider;

        public ScopeSendPipeSpecificationObserver(ISendScopeProvider sendScopeProvider)
        {
            _sendScopeProvider = sendScopeProvider;
        }
        public void MessageSpecificationCreated<T>(IMessageSendPipeSpecification<T> specification)
            where T : class
        {
            specification.AddPipeSpecification(new ScopeSendPipeSpecification<T>(_sendScopeProvider));
        }
    }
}

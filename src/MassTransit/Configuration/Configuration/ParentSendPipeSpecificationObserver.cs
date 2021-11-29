namespace MassTransit.Configuration
{
    public class ParentSendPipeSpecificationObserver :
        ISendPipeSpecificationObserver
    {
        readonly ISendPipeSpecification _specification;

        public ParentSendPipeSpecificationObserver(ISendPipeSpecification specification)
        {
            _specification = specification;
        }

        public void MessageSpecificationCreated<T>(IMessageSendPipeSpecification<T> specification)
            where T : class
        {
            IMessageSendPipeSpecification<T> messageSpecification = _specification.GetMessageSpecification<T>();

            specification.AddParentMessageSpecification(messageSpecification);
        }
    }
}

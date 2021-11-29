namespace MassTransit.Configuration
{
    public class ParentPublishPipeSpecificationObserver :
        IPublishPipeSpecificationObserver
    {
        readonly IPublishPipeSpecification _specification;

        public ParentPublishPipeSpecificationObserver(IPublishPipeSpecification specification)
        {
            _specification = specification;
        }

        public void MessageSpecificationCreated<T>(IMessagePublishPipeSpecification<T> specification)
            where T : class
        {
            IMessagePublishPipeSpecification<T> messageSpecification = _specification.GetMessageSpecification<T>();

            specification.AddParentMessageSpecification(messageSpecification);
        }
    }
}

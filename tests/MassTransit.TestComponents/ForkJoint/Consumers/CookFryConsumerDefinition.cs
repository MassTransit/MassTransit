namespace MassTransit.TestComponents.ForkJoint.Consumers
{
    using Definition;


    public class CookFryConsumerDefinition :
        ConsumerDefinition<CookFryConsumer>
    {
        public CookFryConsumerDefinition()
        {
            ConcurrentMessageLimit = 32;
        }
    }
}

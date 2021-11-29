namespace MassTransit.TestFramework.ForkJoint.Consumers
{
    public class CookFryConsumerDefinition :
        ConsumerDefinition<CookFryConsumer>
    {
        public CookFryConsumerDefinition()
        {
            ConcurrentMessageLimit = 32;
        }
    }
}

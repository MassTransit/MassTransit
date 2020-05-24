namespace MassTransit.KafkaIntegration
{
    using Registration;
    using Subscriptions;


    public static class KafkaBusAttachmentExtensions
    {
        public static void ConnectKafka(this IBusInstance busInstance, params IKafkaSubscription[] subscriptions)
        {
            busInstance.Connect(new KafkaBusAttachment(subscriptions));
        }
    }
}

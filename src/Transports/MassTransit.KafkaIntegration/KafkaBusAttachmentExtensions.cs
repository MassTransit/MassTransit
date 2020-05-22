namespace MassTransit.KafkaIntegration
{
    using Registration;
    using Subscriptions;


    public static class KafkaBusAttachmentExtensions
    {
        public static void ConnectKafka(this IBusInstance busInstance, params IKafkaConsumer[] consumers)
        {
            busInstance.Connect(new KafkaBusAttachment(consumers));
        }
    }
}

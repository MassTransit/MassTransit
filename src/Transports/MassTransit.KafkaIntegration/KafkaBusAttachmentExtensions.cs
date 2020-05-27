namespace MassTransit.KafkaIntegration
{
    using Attachments;
    using Registration;
    using Subscriptions;


    public static class KafkaBusAttachmentExtensions
    {
        public static void ConnectKafka(this IBusInstance busInstance, BusAttachmentObservable observers, params IKafkaReceiveEndpoint[] endpoints)
        {
            var attachment = new KafkaBaseBusAttachment(endpoints, observers);
            busInstance.Connect(attachment);
        }
    }
}

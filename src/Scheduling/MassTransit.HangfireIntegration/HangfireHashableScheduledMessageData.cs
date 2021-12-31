namespace MassTransit.HangfireIntegration
{
    using Scheduling;


    interface IHashable
    {
        string HashId { get; }
    }


    class HangfireHashableScheduledMessageData :
        HangfireScheduledMessageData,
        IHashable
    {
        public string HashId { get; set; }

        public static HangfireHashableScheduledMessageData Create(ConsumeContext<ScheduleMessage> context, string hashId)
        {
            var message = new HangfireHashableScheduledMessageData { HashId = hashId };

            SetBaseProperties(message, context, context.Message.Destination, context.Message.CorrelationId);

            return message;
        }
    }
}

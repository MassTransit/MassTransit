namespace MassTransit.AmazonSqsTransport.TopicSubscription
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Amazon.SimpleNotificationService;
    using Amazon.Lambda.SNSEvents;
    using Amazon.SimpleNotificationService.Model;


    public class TopicSubscription
    {
        readonly IAmazonSimpleNotificationService _client;

        public TopicSubscription() : this(new AmazonSimpleNotificationServiceClient()) {}

        public TopicSubscription(IAmazonSimpleNotificationService client)
        {
            _client = client ?? new AmazonSimpleNotificationServiceClient();
        }

        public async Task Handler(SNSEvent snsEvent)
        {
            foreach (var record in snsEvent.Records)
            {
                var snsRecord = record.Sns;

                await _client.PublishAsync(new PublishRequest
                {
                    Message = snsRecord.Message,
                    MessageAttributes = Map(snsRecord.MessageAttributes),
                    TopicArn = Environment.GetEnvironmentVariable("PUBLISH_TOPIC_ARN")
                });
            }
        }

        public Dictionary<string, MessageAttributeValue> Map(IDictionary<string, SNSEvent.MessageAttribute> attributes) =>
            attributes?.ToDictionary(
                x => x.Key,
                x => new MessageAttributeValue
                {
                    DataType = x.Value.Type,
                    StringValue = x.Value.Value
                });
    }
}

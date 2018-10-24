namespace MassTransit.AmazonSqsTransport.TopicSubscription.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Amazon.Lambda.SNSEvents;
    using Amazon.SimpleNotificationService;
    using Amazon.SimpleNotificationService.Model;
    using NSubstitute;
    using Shouldly;
    using Xunit;


    public class TopicSubscriptionTests
    {
        [Fact]
        public void FunctionTest()
        {
            var client = Substitute.For<IAmazonSimpleNotificationService>();

            var topicSubscription = new TopicSubscription(client);

            var snsEvent = new SNSEvent
            {
                Records = new [] {
                    new SNSEvent.SNSRecord
                    {
                        EventSource = Guid.NewGuid().ToString(),
                        EventSubscriptionArn = Guid.NewGuid().ToString(),
                        Sns = new SNSEvent.SNSMessage
                        {
                            MessageAttributes = new Dictionary<string, SNSEvent.MessageAttribute>(),
                            Message = $"Test {Guid.NewGuid()}"
                        }
                    }
                }
            };

            Func<Task> request = () => topicSubscription.Handler(snsEvent);

            request.ShouldNotThrow();

            client
                .Received()
                .PublishAsync(Arg.Any<PublishRequest>());
        }
    }
}

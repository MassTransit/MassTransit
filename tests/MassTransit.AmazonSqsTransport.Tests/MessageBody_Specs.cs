namespace MassTransit.AmazonSqsTransport.Tests
{
    using System.Text.Json;
    using Amazon.SQS.Model;
    using NUnit.Framework;
    using Serialization;


    [TestFixture]
    public class MessageBody_Specs
    {
        [Test]
        public void Should_handle_cross_region_messages()
        {
            var body = new SqsMessageBody(new Message { Body = CrossRegionMessage });

            JsonElement? bodyElement = body.GetJsonElement(SystemTextJsonMessageSerializer.Options);

            var envelope = bodyElement?.Deserialize<MessageEnvelope>(SystemTextJsonMessageSerializer.Options);

            Assert.Multiple(() =>
            {
                Assert.That(envelope, Is.Not.Null);
                Assert.That(envelope.MessageId, Is.EqualTo("00ab0000-6ab3-f8b4-f78c-08db7c8365ff"));
            });
        }

        [Test]
        public void Should_handle_intra_region_messages()
        {
            var body = new SqsMessageBody(new Message { Body = InRegionMessage });

            JsonElement? bodyElement = body.GetJsonElement(SystemTextJsonMessageSerializer.Options);

            var envelope = bodyElement?.Deserialize<MessageEnvelope>(SystemTextJsonMessageSerializer.Options);

            Assert.Multiple(() =>
            {
                Assert.That(envelope, Is.Not.Null);
                Assert.That(envelope.MessageId, Is.EqualTo("00ab0000-6ab3-f8b4-f78c-08db7c8365ff"));
            });
        }

        const string InRegionMessage = """
            {
                "messageId": "00ab0000-6ab3-f8b4-f78c-08db7c8365ff",
                "requestId": null,
                "correlationId": null,
                "conversationId": "00ab0000-6ab3-f8b4-739e-08db7c83660f",
                "initiatorId": null,
                "sourceAddress": "amazonsqs://us-east-1/some-namespace/some_address?durable=false&autodelete=true",
                "destinationAddress": "amazonsqs://us-east-1/some-namespace_TestMessage.fifo?type=topic",
                "responseAddress": null,
                "faultAddress": null,
                "messageType": [
                "urn:message:TestSerialization:TestMessage"
                ],
                "message": {
                    "name": "Hello world!"
                },
                "expirationTime": null,
                "sentTime": "2023-07-04T11:39:59.689102Z",
                "headers": {},
                "host":
                {
                    "machineName": "XXXX",
                    "processName": "Publisher",
                    "processId": 43776,
                    "assembly": "Publisher",
                    "assemblyVersion": "1.0.0.0",
                    "frameworkVersion": "6.0.14",
                    "massTransitVersion": "8.0.16.0",
                    "operatingSystemVersion": "Microsoft Windows NT 10.0.19044.0"
                }
            }
            """;

        const string CrossRegionMessage = """
            {
                "Type": "Notification",
                "MessageId": "00ab0000-6ab3-f8b4-f78c-08db7c8365ff",
                "SequenceNumber": "10000000000000127000",
                "TopicArn": "arn:aws:sns:eu-west-1:000696323999:some-namespace_TestMessage.fifo",
                "Message": "{\r\n  \"messageId\": \"00ab0000-6ab3-f8b4-f78c-08db7c8365ff\",\r\n  \"requestId\": null,\r\n  \"correlationId\": null,\r\n  \"conversationId\": \"00ab0000-6ab3-f8b4-739e-08db7c83660f\",\r\n  \"initiatorId\": null,\r\n  \"sourceAddress\": \"amazonsqs://us-east-1/some-namespace/some_address?durable=false&autodelete=true\",\r\n  \"destinationAddress\": \"amazonsqs://us-east-1/some-namespace_TestMessage.fifo?type=topic\",\r\n  \"responseAddress\": null,\r\n  \"faultAddress\": null,\r\n  \"messageType\": [\r\n    \"urn:message:TestSerialization:TestMessage\"\r\n  ],\r\n  \"message\": {\r\n    \"name\": \"Hello world!\"\r\n  },\r\n  \"expirationTime\": null,\r\n  \"sentTime\": \"2023-07-04T11:39:59.689102Z\",\r\n  \"headers\": {},\r\n  \"host\": {\r\n    \"machineName\": \"XXXX\",\r\n    \"processName\": \"Publisher\",\r\n    \"processId\": 43776,\r\n    \"assembly\": \"Publisher\",\r\n    \"assemblyVersion\": \"1.0.0.0\",\r\n    \"frameworkVersion\": \"6.0.14\",\r\n    \"massTransitVersion\": \"8.0.16.0\",\r\n    \"operatingSystemVersion\": \"Microsoft Windows NT 10.0.19044.0\"\r\n  }\r\n}",
                "Timestamp": "2023-07-04T06:49:35.120Z",
                "UnsubscribeURL": "https://sns.eu-west-1.amazonaws.com/?Action=Unsubscribe&SubscriptionArn=arn:aws:sns:eu-west-1:000696323999:some-namespace_TestMessage.fifo:16dc9aa0-4cc9-43eb-b0a5-48e56d5a0565",
                "MessageAttributes":
                {
                    "Content-Type":
                    {
                        "Type": "String",
                        "Value": "application/vnd.masstransit+json"
                    }
                }
            }
            """;
    }
}


namespace TestSerialization
{
    public class TestMessage
    {
        public string Name { get; set; }
    }
}

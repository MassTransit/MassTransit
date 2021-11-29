namespace MassTransit.ActiveMqTransport.Tests
{
    using System.Threading.Tasks;
    using NUnit.Framework;
    using Serialization;
    using TestMessages;
    using Transports;


    [TestFixture]
    public class When_the_serialized_message_is_invalid :
        ActiveMqTestFixture
    {
        [Test]
        public async Task Should_fault()
        {
            Task<ConsumeContext<ReceiveFault>> receiveFault = await ConnectPublishHandler<ReceiveFault>();

            await InputQueueSendEndpoint.Send<SubmitOrder>(new { }, context => ApplyStaticMessageToContext(context, Fail));

            await receiveFault;
        }

        void ApplyStaticMessageToContext(SendContext<SubmitOrder> context, string body)
        {
            context.Serializer = new CopyBodySerializer(SystemTextJsonMessageSerializer.JsonContentType, new StringMessageBody(body));
        }

        protected override void ConfigureActiveMqReceiveEndpoint(IActiveMqReceiveEndpointConfigurator configurator)
        {
            Handled<SubmitOrder>(configurator);
        }

        const string Fail = @"
{
    ""messageId"": ""7a000000-9a3c-0005-1813-08d7e7027a0c"",
    ""conversationId"": ""7a000000-9a3c-0005-ed0b-08d7e7027a10"",
    ""sourceAddress"": ""loopback://localhost/ULPC15S12C_SampleApi_bus_xeyyyyr48oyymi95bdm6qyucnd?temporary=true"",
    ""destinationAddress"": ""loopback://localhost/input_queue"",
    ""messageType"": [
        ""urn:message:MassTransit.ActiveMqTransport.Tests.TestMessages:SubmitOrder""
    ],
    ""message"": {
        ""orderId"": ""843d0cea-68d8-43f6-8e23-1141cbce939c"",
        ""timestamp"": ""2020-04-22T21:16:51.8263213Z"",
        ""customerNumber"": false,
        ""forms"": [
            {
                ""formData"": {
                    ""_id"": """",
                    ""schema"": """",
                    ""formType"": """",
                    ""pageNo"": """",
                    ""metadata"": {
                        ""docId"": """",
                        ""pageNo"": false,
                        ""claimNumber"": """",
                        ""claimId"": """",
                        ""clientName"": """",
                        ""clientId"": """",
                        ""createdDateTime"": """"
                    }
                }
            }
        ]
    },
    ""sentTime"": ""2020-04-22T21:16:51.9778323Z"",
    ""headers"": {
        ""MT-Activity-Id"": ""00-1e03f9b1b84f3d40b7087f2633fadd4a-38ac2fc84b035447-00""
    },
    ""host"": {
        ""machineName"": ""ULPC15S12C"",
        ""processName"": ""Sample.Api"",
        ""processId"": 25968,
        ""assembly"": ""Sample.Api"",
        ""assemblyVersion"": ""1.0.0.0"",
        ""frameworkVersion"": ""3.1.3"",
        ""massTransitVersion"": ""6.2.5.0"",
        ""operatingSystemVersion"": ""Microsoft Windows NT 6.2.9200.0""
    }
}";
    }


    namespace TestMessages
    {
        using System;
        using System.Collections.Generic;
        using Newtonsoft.Json;


        public interface MetaData
        {
            int PageNo { get; } // if pass "false" in JSON for this, nothing happens
        }


        public interface FormData
        {
            MetaData MetaData { get; }
        }


        public interface Forms
        {
            FormData FormData { get; }
        }


        public interface SubmitOrder
        {
            Guid OrderId { get; }
            DateTime Timestamp { get; }

            // this works
            [JsonProperty("CustomerNumber", Required = Required.DisallowNull,
                NullValueHandling = NullValueHandling.Ignore)]
            int CustomerNumber { get; }

            string PaymentCardNumber { get; }

            MessageData<string> Notes { get; }

            List<Forms> Forms { get; }
        }
    }
}

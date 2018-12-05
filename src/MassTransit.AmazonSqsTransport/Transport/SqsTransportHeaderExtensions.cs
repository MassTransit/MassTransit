namespace MassTransit.AmazonSqsTransport.Transport
{
    using System;
    using System.Collections.Generic;
    using Amazon.SQS.Model;


    public static class SqsTransportHeaderExtensions
    {
        public static void Set(this IDictionary<string, MessageAttributeValue> attributes, SendHeaders sendHeaders)
        {
            attributes.SetTextHeaders(sendHeaders, (key, text) => new MessageAttributeValue
            {
                StringValue = text,
                DataType = "String"
            });
        }

        public static void Set(this IDictionary<string, MessageAttributeValue> attributes, string key, string value)
        {
            attributes[key] = new MessageAttributeValue
            {
                DataType = "String",
                StringValue = value
            };
        }

        public static void Set(this IDictionary<string, MessageAttributeValue> attributes, string key, Guid? value)
        {
            if (value.HasValue)
            {
                attributes[key] = new MessageAttributeValue
                {
                    DataType = "String",
                    StringValue = value.ToString()
                };
            }
        }

        public static void Set(this IDictionary<string, MessageAttributeValue> attributes, string key, TimeSpan? value)
        {
            if (value.HasValue)
            {
                attributes[key] = new MessageAttributeValue
                {
                    DataType = "String",
                    StringValue = value.Value.TotalMilliseconds.ToString("F0")
                };
            }
        }
    }
}
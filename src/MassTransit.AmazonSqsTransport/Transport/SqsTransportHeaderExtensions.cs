namespace MassTransit.AmazonSqsTransport.Transport
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using Amazon.SQS.Model;


    public static class SqsTransportHeaderExtensions
    {
        public static void Set(this IDictionary<string, MessageAttributeValue> attributes, SendHeaders sendHeaders)
        {
            KeyValuePair<string, object>[] headers = sendHeaders.GetAll()
                .Where(x => x.Value != null && (x.Value is string || IntrospectionExtensions.GetTypeInfo(x.Value.GetType()).IsValueType))
                .ToArray();

            foreach (KeyValuePair<string, object> header in headers)
            {
                if (attributes.ContainsKey(header.Key))
                    continue;

                attributes[header.Key] = new MessageAttributeValue
                {
                    StringValue = header.Value.ToString(),
                    DataType = "String"
                };
            }
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
#nullable disable
namespace MassTransit
{
    using System;
    using System.Collections.Generic;
    using System.Linq;


    public class FutureMessage
    {
        public FutureMessage()
        {
        }

        public FutureMessage(IDictionary<string, object> message, string[] supportedMessageTypes)
        {
            Message = message;
            SupportedMessageTypes = supportedMessageTypes;
        }

        public IDictionary<string, object> Message { get; set; }

        public string[] SupportedMessageTypes { get; set; }

        public bool HasMessageType(Type messageType)
        {
            var typeUrn = MessageUrn.ForTypeString(messageType);

            return SupportedMessageTypes?.Any(x => typeUrn.Equals(x, StringComparison.OrdinalIgnoreCase)) ?? false;
        }

        public bool HasMessageType<T>()
            where T : class
        {
            var typeUrn = MessageUrn.ForTypeString<T>();

            return SupportedMessageTypes?.Any(x => typeUrn.Equals(x, StringComparison.OrdinalIgnoreCase)) ?? false;
        }
    }
}

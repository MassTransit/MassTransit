
namespace MassTransit.AzureServiceBusTransport
{
    using System.Collections.Generic;

    internal class ServiceBusHeaders
    {
        const int MAX_HEADER_LENGTH_BYTES = 32767;
        const int MAX_HEADER_LENGTH = (MAX_HEADER_LENGTH_BYTES / sizeof(char)) - 1;

        public static void TruncateFaultHeaders(IDictionary<string, object> headers)
        {
            if (headers.TryGetValue(MessageHeaders.FaultMessage, out var faultMessage)
                && faultMessage is string message
                && message.Length > MAX_HEADER_LENGTH)
            {
                headers[MessageHeaders.FaultMessage] = message.Substring(0, MAX_HEADER_LENGTH - 3) + "...";
            }
        }
    }
}

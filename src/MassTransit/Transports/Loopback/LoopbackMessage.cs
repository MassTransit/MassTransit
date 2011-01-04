namespace MassTransit.Transports
{
    using System.IO;

    public class LoopbackMessage
    {
        public LoopbackMessage(MemoryStream stream, string messageId, string label)
        {
            Stream = stream;
            Label = label;
            MessageId = messageId;
        }

        public MemoryStream Stream;
        public string MessageId;
        public string Label;
    }
}
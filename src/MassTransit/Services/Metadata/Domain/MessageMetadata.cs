namespace MassTransit.Services.Metadata.Domain
{
    using System.Collections.Generic;
    using Messages;

    public class MessageMetadata
    {
        public MessageMetadata()
        {
            Children = new List<MessageMetadata>();
        }

        public string Name { get; set; }
        public string DotNetType { get; set; }
        public string Owner { get; set; }
        public string Notes { get; set; }

        public MessageMetadata Parent { get; set; }
        public IList<MessageMetadata> Children { get; set; }

        public static MessageMetadata FromDefinition(MessageDefinition message)
        {
            return new MessageMetadata();
        }
    }
}
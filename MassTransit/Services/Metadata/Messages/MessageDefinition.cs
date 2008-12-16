namespace MassTransit.Services.Metadata.Messages
{
    using System;
    using System.Collections.Generic;

    [Serializable]
    public class MessageDefinition
    {
        public MessageDefinition()
        {
            Children = new List<MessageDefinition>();
        }

        public string Name { get; set; }
        public string DotNetType { get; set; }
        public MessageDefinition Parent { get; set; }
        public IList<MessageDefinition> Children { get; set; }

    }
}
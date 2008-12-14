namespace MassTransit.Metadata
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using Messages;

    public class MetadataExtracter
    {
        private readonly Dictionary<Type, MessageModel> _messageHashes;
        private readonly IServiceBus _bus;

        public MetadataExtracter(IServiceBus bus)
        {
            _messageHashes = new Dictionary<Type, MessageModel>();
            _bus = bus;
        }

        private MessageModel Extract(object message)
        {
            Type messageType = message.GetType();

            var result = new MessageModel
                             {
                                 Name = messageType.Name,
                                 Assembly = messageType.Assembly.GetName().FullName
                             };

            foreach (PropertyInfo info in messageType.GetProperties())
            {
                var member = new MemberModel
                                 {
                                     Name = info.Name,
                                     ValueType = info.PropertyType.Name,
                                 };

                //TODO: Needs to walk the entire object graph not just one level deep

                result.Members.Add(member);
            }


            return result;
        }

        public void ExtractAndPublish(object message)
        {
            var messageType = message.GetType();
            if (_messageHashes.ContainsKey(messageType))
                return;

            var metadata = Extract(message);

            _messageHashes.Add(messageType, metadata);

            _bus.Publish(new Metadata(metadata));
        }
    }
}
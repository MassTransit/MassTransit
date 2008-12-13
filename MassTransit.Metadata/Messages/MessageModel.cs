namespace MassTransit.Metadata.Messages
{
    using System;
    using System.Collections.Generic;

    [Serializable]
    public class MessageModel
    {
        public string Name { get; set; }
        public string Assembly { get; set; }

        public IList<MemberModel> Members { get; private set; }

        public MessageModel()
        {
            Members = new List<MemberModel>();
        }
    }
}
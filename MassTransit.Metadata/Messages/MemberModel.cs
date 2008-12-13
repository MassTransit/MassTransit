namespace MassTransit.Metadata.Messages
{
    using System;

    [Serializable]
    public class MemberModel
    {
        public string Name { get; set; }
        public string ValueType { get; set; }
    }
}
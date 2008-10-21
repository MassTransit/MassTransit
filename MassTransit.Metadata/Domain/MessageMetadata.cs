namespace MassTransit.Metadata.Domain
{
    using System.Collections.Generic;

    public class MessageMetadata
    {
        public string Name { get; set; }
        public string Assembly { get; set; }
        public string Notes { get; set; }
        public string Owner { get; set; }
        public IList<MemberMetadata> Members { get; set; }
    }
}
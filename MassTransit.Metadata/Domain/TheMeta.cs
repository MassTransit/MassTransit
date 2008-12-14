namespace MassTransit.Metadata.Domain
{
    using System.Collections.Generic;

    public class TheMeta
    {
        public string Name { get; set; }
        public string DotNetType { get; set; }
        public string Owner { get; set; }
        public string Notes { get; set; }

        public TheMeta Parent { get; set; }
        public IList<TheMeta> Children { get; set; }
    }
}
namespace MassTransit.Licensing
{
    using System;
    using System.Collections.Generic;


    public class LicenseProduct
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
        public DateTime? Expires { get; set; }
        public Dictionary<string, LicenseFeature>? Features { get; set; }
    }
}

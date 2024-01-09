namespace MassTransit.Licensing
{
    using System;
    using System.Collections.Generic;


    public class LicenseInfo
    {
        public LicenseContact? Contact { get; set; }
        public LicenseCustomer? Customer { get; set; }

        public Dictionary<string, LicenseProduct>? Products { get; set; }

        public DateTime Created { get; set; }
        public DateTime Expires { get; set; }
    }
}

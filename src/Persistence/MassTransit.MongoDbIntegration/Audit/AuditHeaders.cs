namespace MassTransit.MongoDbIntegration.Audit
{
    using System.Collections.Generic;


    public class AuditHeaders : Dictionary<string, string>
    {
        AuditHeaders(IDictionary<string, string> dictionary)
            : base(dictionary)
        {
        }

        public AuditHeaders()
        {
            // Serialization constructor
        }

        internal static AuditHeaders FromDictionary(IDictionary<string, string> dictionary)
        {
            return dictionary == null
                ? new AuditHeaders()
                : new AuditHeaders(dictionary);
        }
    }
}

namespace MassTransit.AmazonSqsTransport.Topology.Configuration.Configurators
{
    using System.Collections.Generic;


    public abstract class EntityConfigurator
    {
        protected EntityConfigurator(string entityName, bool durable = true, bool autoDelete = false)
        {
            EntityName = entityName;
            Durable = durable;
            AutoDelete = autoDelete;
        }

        public bool Durable { get; set; }
        public bool AutoDelete { get; set; }
        public string EntityName { get; set; }

        protected virtual IEnumerable<string> GetQueryStringOptions()
        {
            if (!Durable)
                yield return "durable=false";

            if (AutoDelete)
                yield return "autodelete=true";
        }
    }
}

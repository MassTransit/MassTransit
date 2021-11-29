namespace MassTransit.AmazonSqsTransport
{
    using System.Collections.Generic;


    public class TopicInfo
    {
        readonly IDictionary<string, string> _attributes;

        public TopicInfo(string entityName, string arn, IDictionary<string, string> attributes)
        {
            _attributes = attributes;

            EntityName = entityName;
            Arn = arn;
        }

        public string EntityName { get; }
        public string Arn { get; }
    }
}

namespace MassTransit.AmazonSqsTransport
{
    public class TopicInfo
    {
        public TopicInfo(string entityName, string arn)
        {
            EntityName = entityName;
            Arn = arn;
        }

        public string EntityName { get; }
        public string Arn { get; }
    }
}

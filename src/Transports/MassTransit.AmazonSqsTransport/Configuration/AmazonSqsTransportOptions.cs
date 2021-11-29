namespace MassTransit
{
    public class AmazonSqsTransportOptions
    {
        public string Region { get; set; }
        public string Scope { get; set; }
        public string AccessKey { get; set; }
        public string SecretKey { get; set; }
    }
}

namespace MassTransit.AzureCosmos.Saga
{
    public class SagaETag
    {
        public SagaETag(string eTag)
        {
            ETag = eTag;
        }

        public string ETag { get; }
    }
}

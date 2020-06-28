namespace MassTransit.Azure.Cosmos.Saga.Context
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

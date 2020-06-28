namespace MassTransit.Azure.Table.Contexts
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

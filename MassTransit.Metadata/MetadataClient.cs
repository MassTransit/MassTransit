namespace MassTransit.Metadata
{
    using Messages;

    public class MetadataClient :
        Consumes<object>.All
    {
        //hooks into the pipeline to get shiz done
        private readonly MetadataExtracter _extractor;

        public MetadataClient(MetadataExtracter extractor)
        {
            _extractor = extractor;
        }

        public void Consume(object message)
        {
            _extractor.ExtractAndPublish(message);
        }
    }
}
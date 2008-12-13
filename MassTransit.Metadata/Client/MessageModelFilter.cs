namespace MassTransit.Metadata.Client
{
    public class MessageModelFilter :
        WireTap<object>
    {
        public MessageModelFilter(MetadataExtracter extractor) :
            base("Intercept messages for metadat'ing", (sink) => sink, (msg) => true, (message)=>extractor.ExtractAndPublish(message))
        {
            
        }
    }
}
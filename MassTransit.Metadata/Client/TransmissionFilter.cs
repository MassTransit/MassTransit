namespace MassTransit.Metadata.Client
{
    public class TransmissionFilter :
        WireTap<object>
    {
        public TransmissionFilter(MetadataExtracter extractor) :
            base("Intercept messages for transmission", (sink) => sink, (msg) => true, (message)=>extractor.NoteAndPublish(message))
        {
            
        }
    }
}
namespace MassTransit.LegacySupport.SerializationCustomization
{
    using System;
    using System.Runtime.Serialization;

    public class LegacySurrogateSelector :
        ISurrogateSelector
    {
        public void ChainSelector(ISurrogateSelector selector)
        {
            throw new NotImplementedException();
        }

        public ISerializationSurrogate GetSurrogate(Type type, StreamingContext context, out ISurrogateSelector selector)
        {
            throw new NotImplementedException();
        }

        public ISurrogateSelector GetNextSelector()
        {
            throw new NotImplementedException();
        }
    }
}
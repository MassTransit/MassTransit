namespace MassTransit.LegacySupport.SerializationCustomization
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.Serialization;

    public class LegacySurrogateSelector :
        ISurrogateSelector
    {
        readonly SurrogateSelector _inner = new SurrogateSelector();
        readonly List<LegacySurrogate> _surrogates = new List<LegacySurrogate>();

        public void ChainSelector(ISurrogateSelector selector)
        {
            _inner.ChainSelector(selector);
        }

        public ISerializationSurrogate GetSurrogate(Type type, StreamingContext context, out ISurrogateSelector selector)
        {
            if (_surrogates.Any(x => x.SurrogateType == type))
            {
                selector = this;
                return _surrogates.First(x => x.SurrogateType == type);
            }


            return _inner.GetSurrogate(type, context, out selector);
        }

        public ISurrogateSelector GetNextSelector()
        {
            return _inner.GetNextSelector();
        }

        public void AddSurrogate(LegacySurrogate surrogate)
        {
            _surrogates.Add(surrogate);
        }
    }
}
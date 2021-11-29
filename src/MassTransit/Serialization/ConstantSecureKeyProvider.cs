namespace MassTransit.Serialization
{
    public class ConstantSecureKeyProvider :
        ISecureKeyProvider
    {
        readonly byte[] _key;

        public ConstantSecureKeyProvider(byte[] key)
        {
            _key = key;
        }

        public void Probe(ProbeContext context)
        {
            context.Add("key", "constant");
        }

        public byte[] GetKey(Headers headers)
        {
            return _key;
        }
    }
}

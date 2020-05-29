namespace MassTransit.Serialization
{
    using GreenPipes;


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

        public byte[] GetKey(ReceiveContext receiveContext)
        {
            return GetKey();
        }

        public byte[] GetKey(SendContext sendContext)
        {
            return GetKey();
        }

        byte[] GetKey()
        {
            return _key;
        }
    }
}

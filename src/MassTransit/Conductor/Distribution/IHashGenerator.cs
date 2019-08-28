namespace MassTransit.Conductor.Distribution
{
    public interface IHashGenerator
    {
        uint Hash(string s);
        uint Hash(byte[] data);
        uint Hash(byte[] data, int offset, uint count, uint seed);
    }
}

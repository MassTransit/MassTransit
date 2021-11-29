namespace MassTransit.Middleware
{
    /// <summary>
    /// Generates a hash of the input data for partitioning purposes
    /// </summary>
    public interface IHashGenerator
    {
        uint Hash(byte[] data);
    }
}

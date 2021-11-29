namespace MassTransit.Testing.Implementations
{
    /// <summary>
    /// Represents a resource which may be signaled.
    /// </summary>
    public interface ISignalResource
    {
        void Signal();
    }
}

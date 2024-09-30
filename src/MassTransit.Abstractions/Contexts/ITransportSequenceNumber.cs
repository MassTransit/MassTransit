namespace MassTransit
{
    public interface ITransportSequenceNumber
    {
        ulong? SequenceNumber { get; }
    }
}

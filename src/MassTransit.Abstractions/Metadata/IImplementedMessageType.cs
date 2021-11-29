namespace MassTransit.Metadata
{
    public interface IImplementedMessageType
    {
        void ImplementsMessageType<T>(bool direct)
            where T : class;
    }
}

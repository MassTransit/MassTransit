namespace MassTransit.Serialization.JsonConverters
{
    public interface IMessageDataReference
    {
        string Text { set; }
        byte[] Data { set; }
    }
}

namespace MassTransit.MessageData
{
    public interface IMessageDataReference
    {
        string Text { set; }
        byte[] Data { set; }
    }
}

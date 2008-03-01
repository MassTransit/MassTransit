namespace MassTransit.ServiceBus.Formatters
{
    using System.IO;

    public interface IMessageFormatter
    {
        IMessageBody Serialize(IEnvelope env, IMessageBody body);
        IEnvelope Deserialize(IMessageBody messageBody);
    }

    public interface IMessageBody
    {
        object Body { get; set; }
        Stream BodyStream { get; set; }
    }
}
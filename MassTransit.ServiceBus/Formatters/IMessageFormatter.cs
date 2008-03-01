namespace MassTransit.ServiceBus.Formatters
{
    public interface IMessageFormatter
    {
        object Serialize(IEnvelope env);
        IEnvelope Deserialize(object messageBody);
    }
}
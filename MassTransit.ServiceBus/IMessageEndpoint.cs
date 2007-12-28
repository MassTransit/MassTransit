namespace MassTransit.ServiceBus
{
    public interface IMessageEndpoint<T> : 
		IEndpoint where T : IMessage
    {
        event MessageHandler<T> MessageReceived;
    }

    //we should be careful with a bus instance here I think. It has the wrong methods really. well depending on if this is the remote bus or not.
    public delegate void MessageHandler<T>(IServiceBus bus, IEnvelope envelope, T message);
    public delegate void MesageHandler<T>(MessageContext<T> cxt) where T : IMessage;
}
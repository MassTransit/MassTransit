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


	// NOTE: Here is my sense of this. Normally this is object sender, which I find very, very confusing when trying to read the code
	// NOTE: sender could be (a) the event sender, (b) the message source, or (c) the originator
	// NOTE: So how to make that happy from a "soluability" perspective is still out there.
	// NOTE: MessageContext<T> having a method Reply(), is that something we want to do?
    // NOTE: DRU: YEAH!
}
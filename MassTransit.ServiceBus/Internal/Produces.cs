namespace MassTransit.ServiceBus.Internal
{
    public interface Produces<TMessage> where TMessage : class
    {
        void Attach(Consumes<TMessage>.All consumer);
        void Detach(Consumes<TMessage>.All consumer);
    }
}
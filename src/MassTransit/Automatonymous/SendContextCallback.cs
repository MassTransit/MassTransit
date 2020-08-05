namespace Automatonymous
{
    using MassTransit;


    public delegate void SendContextCallback<in TInstance, in TData, in TMessage>(ConsumeEventContext<TInstance, TData> context,
        SendContext<TMessage> sendContext)
        where TMessage : class
        where TData : class;


    public delegate void SendContextCallback<in TInstance, in TMessage>(ConsumeEventContext<TInstance> context, SendContext<TMessage> sendContext)
        where TMessage : class;
}

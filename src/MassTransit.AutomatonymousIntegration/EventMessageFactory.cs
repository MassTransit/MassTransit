namespace Automatonymous
{
    public delegate TMessage EventMessageFactory<in TInstance, out TMessage>(ConsumeEventContext<TInstance> context);


    public delegate TMessage EventMessageFactory<in TInstance, in TData, out TMessage>(ConsumeEventContext<TInstance, TData> context)
        where TData : class;
}

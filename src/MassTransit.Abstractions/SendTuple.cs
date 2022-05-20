namespace MassTransit
{
    /// <summary>
    /// Combines a message and a pipe which can be used to send/publish the message
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public readonly struct SendTuple<T>
        where T : class
    {
        public readonly T Message;
        public readonly IPipe<SendContext<T>> Pipe;

        public SendTuple(T message, IPipe<SendContext<T>>? pipe)
        {
            Message = message;
            Pipe = pipe != null && pipe.IsNotEmpty() ? pipe : MassTransit.Pipe.Empty<SendContext<T>>();
        }

        public SendTuple(T message)
        {
            Message = message;
            Pipe = MassTransit.Pipe.Empty<SendContext<T>>();
        }

        public void Deconstruct(out T message, out IPipe<SendContext<T>> pipe)
        {
            message = Message;
            pipe = Pipe;
        }
    }
}

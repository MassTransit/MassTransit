namespace MassTransit.Testing
{
    using System.Linq;
    using System.Threading;


    public static class AsyncMessageListExtensions
    {
        public static int Count<TElement>(this IAsyncElementList<TElement> elements, CancellationToken cancellationToken = default)
            where TElement : class, IAsyncListElement
        {
            return elements.Select(x => true, cancellationToken).Count();
        }

        public static void Deconstruct(this ISentMessage sent, out object message, out SendContext context)
        {
            context = sent.Context;
            message = sent.MessageObject;
        }

        public static void Deconstruct<TMessage>(this ISentMessage<TMessage> sent, out TMessage message, out SendContext context)
            where TMessage : class
        {
            context = sent.Context;
            message = sent.Context.Message;
        }
    }
}

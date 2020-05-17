namespace MassTransit.Testing
{
    public class ReceivedMessageFilterSet :
        FilterSet<IReceivedMessage>
    {
        public ReceivedMessageFilterSet Add<T>()
            where T : class
        {
            static bool Filter(IReceivedMessage element)
            {
                return element is IReceivedMessage<T>;
            }

            Add(Filter);

            return this;
        }

        public ReceivedMessageFilterSet Add<T>(FilterDelegate<IReceivedMessage<T>> filter)
            where T : class
        {
            bool Filter(IReceivedMessage element)
            {
                return element is IReceivedMessage<T> result && filter(result);
            }

            Add(Filter);

            return this;
        }
    }
}

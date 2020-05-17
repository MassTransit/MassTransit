namespace MassTransit.Testing
{
    public class SentMessageFilterSet :
        FilterSet<ISentMessage>
    {
        public SentMessageFilterSet Add<T>()
            where T : class
        {
            static bool Filter(ISentMessage element)
            {
                return element is ISentMessage<T>;
            }

            Add(Filter);

            return this;
        }

        public SentMessageFilterSet Add<T>(FilterDelegate<ISentMessage<T>> filter)
            where T : class
        {
            bool Filter(ISentMessage element)
            {
                return element is ISentMessage<T> result && filter(result);
            }

            Add(Filter);

            return this;
        }
    }
}

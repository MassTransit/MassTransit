namespace MassTransit.Testing
{
    public class PublishedMessageFilterSet :
        FilterSet<IPublishedMessage>
    {
        public PublishedMessageFilterSet Add<T>()
            where T : class
        {
            static bool Filter(IPublishedMessage element)
            {
                return element is IPublishedMessage<T>;
            }

            Add(Filter);

            return this;
        }

        public PublishedMessageFilterSet Add<T>(FilterDelegate<IPublishedMessage<T>> filter)
            where T : class
        {
            bool Filter(IPublishedMessage element)
            {
                return element is IPublishedMessage<T> result && filter(result);
            }

            Add(Filter);

            return this;
        }
    }
}

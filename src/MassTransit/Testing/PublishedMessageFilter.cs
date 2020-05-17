namespace MassTransit.Testing
{
    public class PublishedMessageFilter
    {
        readonly PublishedMessageFilterSet _excludes = new PublishedMessageFilterSet();
        readonly PublishedMessageFilterSet _includes = new PublishedMessageFilterSet();

        public PublishedMessageFilterSet Includes
        {
            get => _includes;
            set { }
        }

        public PublishedMessageFilterSet Excludes
        {
            get => _excludes;
            set { }
        }

        public bool Any(IPublishedMessage element)
        {
            return _includes.Any(element) && _excludes.None(element);
        }

        public bool None(IPublishedMessage element)
        {
            return _includes.None(element);
        }
    }
}

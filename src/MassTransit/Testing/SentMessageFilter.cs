namespace MassTransit.Testing
{
    public class SentMessageFilter
    {
        readonly SentMessageFilterSet _excludes = new SentMessageFilterSet();
        readonly SentMessageFilterSet _includes = new SentMessageFilterSet();

        public SentMessageFilterSet Includes
        {
            get => _includes;
            set { }
        }

        public SentMessageFilterSet Excludes
        {
            get => _excludes;
            set { }
        }

        public bool Any(ISentMessage element)
        {
            return _includes.Any(element) && _excludes.None(element);
        }

        public bool None(ISentMessage element)
        {
            return _includes.None(element);
        }
    }
}

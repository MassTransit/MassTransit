namespace MassTransit.Testing
{
    public class ReceivedMessageFilter
    {
        readonly ReceivedMessageFilterSet _excludes = new ReceivedMessageFilterSet();
        readonly ReceivedMessageFilterSet _includes = new ReceivedMessageFilterSet();

        public ReceivedMessageFilterSet Includes
        {
            get => _includes;
            set { }
        }

        public ReceivedMessageFilterSet Excludes
        {
            get => _excludes;
            set { }
        }

        public bool Any(IReceivedMessage element)
        {
            return _includes.Any(element) && _excludes.None(element);
        }

        public bool None(IReceivedMessage element)
        {
            return _includes.None(element);
        }
    }
}

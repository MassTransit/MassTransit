namespace MassTransit.ServiceBus.Internal
{
    using log4net;

    public class SpecialLoggers
    {
        private readonly ILog _messages = LogManager.GetLogger("MassTransit.Messages");
        private readonly ILog _diagnostics = LogManager.GetLogger("MassTransit.Diagnostics");

        public ILog Messages
        {
            get { return _messages; }
        }

        public ILog Diagnostics
        {
            get { return _diagnostics; }
        }
    }
}
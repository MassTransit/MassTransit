namespace ServerGUI
{
    using log4net;
    using MassTransit.ServiceBus;
    using Messages;

    public class UserAgentSession : 
        Consumes<UserAgentStarted>.All
    {
        private static readonly ILog _log = LogManager.GetLogger(typeof (UserAgentSession));

        public void Consume(UserAgentStarted message)
        {
            _log.DebugFormat("UserAgentStarted");
        }
    }
}
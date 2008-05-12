namespace MassTransit.Patterns.FaultDetection
{
    using System;
    using log4net;
    using MassTransit.ServiceBus;
    using MassTransit.Patterns.FaultDetection.Messages;

    public class Reporter/* :
        IConsume<DownEndpoint>*/
    {
        private ILog _log = LogManager.GetLogger(typeof(Reporter));

        public void Handle(IMessageContext<DownEndpoint> cxt)
        {
            _log.ErrorFormat("Endpoint '{0}' is down", cxt.Envelope.ReturnEndpoint);
        }
    }
}

namespace MassTransit.Patterns.FaultDetection
{
    using System;
    using System.Collections.Generic;
    using MassTransit.ServiceBus;
    using MassTransit.Patterns.FaultDetection.Messages;

    public class Investigator/* :
        IConsume<Suspect>*/
    {
        IServiceBus _bus;

        public Investigator(IServiceBus bus)
        {
            _bus = bus;
        }

        public void Handle(IMessageContext<Suspect> cxt)
        {
            IServiceBusAsyncResult async = _bus.Request<Ping>(cxt.Message.Endpoint, new Ping());
            async.AsyncWaitHandle.WaitOne(3000, true);
            IList<IMessage> msg = async.Messages;

            if (msg.Count > 0)
            {
                //I have an endpoint in a weird state
            }
            else
            {
                //I have confirmed dead endpoint
                _bus.Publish<DownEndpoint>(new DownEndpoint());
            }
        }
    }
}

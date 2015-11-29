namespace MassTransit.Pipeline
{
    using System;
    using System.Threading.Tasks;
    using Context;


    public class PublishPipeProxy :
        ISendPipe
    {
        readonly IPublishPipe _publishPipe;

        public PublishPipeProxy(IPublishPipe publishPipe)
        {
            _publishPipe = publishPipe;
        }

        public Task Send(SendContext context)
        {
            var proxy = new PublishContextProxy(context);

            return _publishPipe.Send(proxy);
        }

        public void Probe(ProbeContext context)
        {
            _publishPipe.Probe(context);
        }

        public ConnectHandle ConnectSendObserver(ISendObserver observer)
        {
            throw new NotImplementedException();
        }

        public ConnectHandle ConnectSendMessageObserver<T>(ISendMessageObserver<T> observer) where T : class
        {
            throw new NotImplementedException();
        }
    }
}
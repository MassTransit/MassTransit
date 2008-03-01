namespace MassTransit.ServiceBus.NMS
{
    using System.IO;
    using Apache.NMS;
    using Formatters;

    public class NmsMessageBody :
        IFormattedBody
    {
        private ISession sess;


        public NmsMessageBody(ISession sess)
        {
            this.sess = sess;
        }

        public object Body
        {
            get { throw new System.NotImplementedException(); }
            set { throw new System.NotImplementedException(); }
        }

        public Stream BodyStream
        {
            get { throw new System.NotImplementedException(); }
            set { throw new System.NotImplementedException(); }
        }
    }
}
using System;
using log4net;

namespace MassTransit.ServiceBus
{
    public class MessageQueueEndpoint :
        IMessageQueueEndpoint
    {
        private static readonly ILog _log = LogManager.GetLogger(typeof (MessageQueueEndpoint));

        private readonly Uri _uri;
        private string _queuePath;

        //private MessageQueue _queue;

        public MessageQueueEndpoint(string uriString)
            : this(new Uri(uriString))
        {
            string hostName = _uri.Host;
            if (string.Compare(hostName, ".") == 0 || string.Compare(hostName, "localhost", true) == 0)
            {
                hostName = Environment.MachineName;
            }

            _queuePath = string.Format(@"{0}\private$\{1}", hostName, _uri.AbsolutePath.Substring(1));
        }

        public MessageQueueEndpoint(Uri uri)
        {
            _uri = uri;

            //_queue = new MessageQueue(QueueName, QueueAccessMode.Send);
        }

        #region IMessageQueueEndpoint Members

        public string QueueName
        {
            get { return _queuePath; }
        }

        public Uri Uri
        {
            get { return _uri; }
        }

        public void Dispose()
        {
            //_queue.Close();
            //_queue.Dispose();
            //_queue = null;
        }

        #endregion

        //private MessageQueueTransactionType GetTransactionType()
        //{
        //    MessageQueueTransactionType tt = MessageQueueTransactionType.None;
        //    if (_queue.Transactional)
        //    {
        //        Check.RequireTransaction(
        //            string.Format(
        //                "The current queue {0} is transactional and this MessageQueueEndpoint is not running in a transaction.",
        //                _uri));

        //        tt = MessageQueueTransactionType.Automatic;
        //    }
        //    return tt;
        //}

        public static implicit operator MessageQueueEndpoint(string queueUri)
        {
            return new MessageQueueEndpoint(queueUri);
        }

        public static implicit operator string(MessageQueueEndpoint endpoint)
        {
            return endpoint.Uri.AbsoluteUri;
        }

        public static IMessageQueueEndpoint FromQueuePath(string path)
        {
            const string prefix = "FORMATNAME:DIRECT=OS:";

            if (path.Length > prefix.Length && path.Substring(0, prefix.Length).ToUpperInvariant() == prefix)
                path = path.Substring(prefix.Length);

            string[] parts = path.Split('\\');

            if(parts.Length != 3)
                throw new ArgumentException("Invalid Queue Path Specified");

            if(string.Compare(parts[1], "private$", true) != 0 )
                throw new ArgumentException("Invalid Queue Path Specified");

            return new MessageQueueEndpoint(string.Format("msmq://{0}/{1}", parts[0], parts[2]));
        }
    }
}
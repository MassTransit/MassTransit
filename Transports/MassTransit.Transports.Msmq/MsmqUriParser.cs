namespace MassTransit.Transports.Msmq
{
    using System;
    using System.Messaging;
    using System.Runtime.Serialization;

    public class MsmqUriParser
    {
        private const string _localhost = "localhost";

        public static ParsingResult Convert(string uriString)
        {
            return Convert(new Uri(uriString));
        }
        public static ParsingResult Convert(Uri uri)
        {
            bool isLocal = false;
            if (uri.AbsolutePath.IndexOf("/", 1) >= 0)
            {
                if (uri.AbsolutePath.IndexOf("public") >= 0)
                    throw new NotSupportedException(string.Format("public queues are not supported (please submit a patch): {0}", uri));

                throw new ParsingException("Queue Endpoints can't have a child folder unless it is 'public' (not supported yet, please submit patch). Good: 'msmq://machinename/queue_name' or 'msmq://machinename/public/queue_name' - Bad: msmq://machinename/round_file/queue_name");
            }


            string localMachineName = Environment.MachineName.ToLowerInvariant();

            string hostName = uri.Host;
            if (string.Compare(hostName, ".") == 0 || string.Compare(hostName, _localhost, true) == 0)
            {
                uri = new Uri("msmq://" + localMachineName + uri.AbsolutePath);
                isLocal = true;
            }
            else
            {
                isLocal = string.Compare(uri.Host, localMachineName, true) == 0;
            }

            var queuePath = string.Format(@"FormatName:DIRECT=OS:{0}\private$\{1}", hostName, uri.AbsolutePath.Substring(1));

            return new ParsingResult {IsLocal = isLocal, QueuePath = queuePath, FullyQualifiedUri = uri};
        }

        public static ParsingResult Convert(MessageQueue queue)
        {
            string path = queue.Path;
            const string prefix = "FormatName:DIRECT=OS:";

            if (path.Length > prefix.Length && path.Substring(0, prefix.Length).ToUpperInvariant() == prefix.ToUpperInvariant())
                path = path.Substring(prefix.Length);

            string[] parts = path.Split('\\');

            if (parts.Length != 3)
                throw new ArgumentException("Invalid Queue Path Specified");

            //Validate parts[1] = private$
            if (string.Compare(parts[1], "private$", true) != 0)
                throw new ArgumentException("Invalid Queue Path Specified");

            string localhost = Environment.MachineName.ToLowerInvariant();
            bool isLocal;
            if (parts[0] == "." || string.Compare("localhost", parts[0], true) == 0)
            {
                parts[0] = localhost;
                isLocal = true;
            }
            else
            {
                parts[0] = parts[0].ToLowerInvariant();
                isLocal = string.Compare(localhost, parts[0], true) == 0;
            }

            var queuePath = string.Format("{0}{1}\\{2}\\{3}", prefix, parts[0], parts[1], parts[2]);
            var uri = new Uri(string.Format("msmq://{0}/{1}", parts[0], parts[2]));

            return new ParsingResult(){IsLocal = isLocal, QueuePath = queuePath, FullyQualifiedUri = uri};
        }
    }

    public class ParsingResult
    {
        public bool IsLocal { get; set; }
        public string QueuePath { get; set; }
        public Uri FullyQualifiedUri { get; set; }
    }

    public class ParsingException :
        Exception
    {
        public ParsingException()
        {
        }

        public ParsingException(string message) : base(message)
        {
        }

        public ParsingException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected ParsingException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
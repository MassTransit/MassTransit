// Copyright 2007-2008 The Apache Software Foundation.
//  
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use 
// this file except in compliance with the License. You may obtain a copy of the 
// License at 
// 
//     http://www.apache.org/licenses/LICENSE-2.0 
// 
// Unless required by applicable law or agreed to in writing, software distributed 
// under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR 
// CONDITIONS OF ANY KIND, either express or implied. See the License for the 
// specific language governing permissions and limitations under the License.
namespace MassTransit.Transports.Nms
{
    using System;
    using System.IO;
    using System.Runtime.Serialization;
    using Apache.NMS;
    using Apache.NMS.ActiveMQ;
    using Exceptions;
    using Internal;
    using log4net;
    using Serialization;

    public class NmsEndpoint :
        IEndpoint
    {

        private static readonly ILog _log = LogManager.GetLogger(typeof (NmsEndpoint));
        private static readonly IMessageSerializer _serializer = new BinaryMessageSerializer();
        private readonly IConnectionFactory _factory;
        private readonly string _queueName;
        private readonly Uri _uri;
        private readonly IConnection _connection;

        public NmsEndpoint(Uri uri)
        {
            _uri = uri;

            UriBuilder queueUri = new UriBuilder("tcp", Uri.Host, Uri.Port);

            _queueName = Uri.AbsolutePath.Substring(1);

            _factory = new ConnectionFactory(queueUri.Uri);

            _connection = _factory.CreateConnection();
            _connection.ExceptionListener += ConnectionExceptionListener;
        }

        void ConnectionExceptionListener(Exception ex)
        {
            _log.Error("NMS threw an exception: ", ex);
        }

        public NmsEndpoint(string uriString)
            : this(new Uri(uriString))
        {
        }

        public Uri Uri
        {
            get { return _uri; }
        }

        public static string Scheme
        {
            get { return "activemq"; }
        }


        public void Send<T>(T message) where T : class
        {
            Send(message, NMSConstants.defaultTimeToLive);
        }

        public void Send<T>(T message, TimeSpan timeToLive) where T : class
        {
            Type messageType = typeof (T);

            using (ISession session = _connection.CreateSession())
            {
                IBytesMessage bm = session.CreateBytesMessage();

                using (MemoryStream mem = new MemoryStream())
                {
                    _serializer.Serialize(mem, message);

                    bm.Content = mem.ToArray();
                }

                if (timeToLive < NMSConstants.defaultTimeToLive)
                    bm.NMSTimeToLive = timeToLive;

                bm.NMSPersistent = true;

                IDestination destination = session.GetQueue(_queueName);

                using (IMessageProducer producer = session.CreateProducer(destination))
                {
                    try
                    {
                        producer.Send(bm);

                        if (SpecialLoggers.Messages.IsInfoEnabled)
                            SpecialLoggers.Messages.InfoFormat("Message {0} Sent To {1}", messageType, Uri);
                    }
                    catch (Exception ex)
                    {
                        throw new EndpointException(this, "Problem with " + Uri, ex);
                    }

                    if (_log.IsDebugEnabled)
                        _log.DebugFormat("Message Sent: Id = {0}, Message Type = {1}", bm.NMSMessageId, messageType);
                }
            }
        }

        public object Receive(TimeSpan timeout)
        {
            try
            {
                using (ISession session = _connection.CreateSession())
                {
                    IDestination destination = session.GetQueue(_queueName);
                    using (IMessageConsumer consumer = session.CreateConsumer(destination))
                    {
                        IMessage message = consumer.Receive(timeout);
                        if (message == null)
                            return null;

                        IBytesMessage bm = message as IBytesMessage;
                        if (bm == null)
                            throw new MessageException(message.GetType(), "Message not a IBytesMessage");

                        try
                        {
                            MemoryStream mem = new MemoryStream(bm.Content, false);

                            object obj = _serializer.Deserialize(mem);

                            return obj;
                        }
                        catch (SerializationException ex)
                        {
                            throw new MessageException(message.GetType(), "An error occurred deserializing a message", ex);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new EndpointException(this, "Receive error occured", ex);
            }
        }

        public object Receive(TimeSpan timeout, Predicate<object> accept)
        {
            using (ISession session = _connection.CreateSession())
            {
                IDestination destination = session.GetQueue(_queueName);
                using (IMessageConsumer consumer = session.CreateConsumer(destination))
                {
                    IMessage message = consumer.Receive(timeout);
                    if (message == null)
                        return null;

                    IBytesMessage bm = message as IBytesMessage;
                    if (bm == null)
                        throw new MessageException(message.GetType(), "Message not a IBytesMessage");

                    try
                    {
                        MemoryStream mem = new MemoryStream(bm.Content, false);

                        object obj = _serializer.Deserialize(mem);

                        if (accept(obj))
                        {
                            if (_log.IsDebugEnabled)
                                _log.DebugFormat("Queue: {0} Received Message Id {1}", _queueName, message.NMSMessageId);

                            if (SpecialLoggers.Messages.IsInfoEnabled)
                                SpecialLoggers.Messages.InfoFormat("RECV:{0}:System.Object:{1}", _queueName, message.NMSMessageId);

                            return obj;
                        }
                        else
                        {
                            if (_log.IsDebugEnabled)
                                _log.DebugFormat("Queue: {0} Skipped Message Id {1}", _queueName, message.NMSMessageId);
                        }
                    }
                    catch (SerializationException ex)
                    {
                        throw new MessageException(typeof (object), "An error occurred deserializing a message", ex);
                    }

                    return null;
                }
            }
        }

        public void Dispose()
        {
            if (_connection != null)
            {
                _connection.ExceptionListener -= ConnectionExceptionListener;
                _connection.Close();
                _connection.Dispose();
            }
        }
    }
}
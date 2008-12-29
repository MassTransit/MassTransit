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
    using System.Collections.Generic;
    using System.IO;
    using System.Runtime.Serialization;
    using Apache.NMS;
    using Apache.NMS.ActiveMQ;
    using Configuration;
    using Exceptions;
    using Internal;
    using log4net;
    using Serialization;

    /// <summary>
    /// The NMS endpoint is used to communicate with systems like ActiveMQ via the Apache.NMS
    /// project
    /// </summary>
    public class NmsEndpoint :
        IEndpoint
    {
        private static readonly ILog _log = LogManager.GetLogger(typeof (NmsEndpoint));
        private readonly IConnectionFactory _factory;
        private readonly string _queueName;
        private readonly IMessageSerializer _serializer;
        private readonly Uri _uri;
        private IConnection _connection;
        private bool _disposed;

        public NmsEndpoint(Uri uri, IMessageSerializer serializer)
        {
            _uri = uri;
            _serializer = serializer;

            var queueUri = new UriBuilder("tcp", Uri.Host, Uri.Port);

            _queueName = Uri.AbsolutePath.Substring(1);

            _factory = new ConnectionFactory(queueUri.Uri);

            _connection = _factory.CreateConnection();
            _connection.ExceptionListener += ConnectionExceptionListener;

            _connection.Start();
        }

        public NmsEndpoint(string uriString, IMessageSerializer serializer)
            : this(new Uri(uriString), serializer)
        {}

        public Uri Uri
        {
            get { return _uri; }
        }

        public void Send<T>(T message) where T : class
        {
            Send(message, NMSConstants.defaultTimeToLive);
        }

        public void Send<T>(T message, TimeSpan timeToLive) where T : class
        {
            if (_disposed) throw new ObjectDisposedException("The object has been disposed");

            try
            {
                using (var session = _connection.CreateSession(AcknowledgementMode.Transactional))
                {
                    var destination = session.GetQueue(_queueName);

                    IBytesMessage bm;
                    using (var producer = session.CreateProducer(destination))
                    {
                        bm = BuildMessage(session, message);

                        if (timeToLive < NMSConstants.defaultTimeToLive)
                            producer.TimeToLive = timeToLive;

                        producer.Persistent = true;

                        producer.Send(bm);
                    }

                    if (SpecialLoggers.Messages.IsInfoEnabled)
                        SpecialLoggers.Messages.InfoFormat("SEND:{0}:{1}:{2}", Uri, typeof (T).Name, bm.NMSMessageId);

                    if (_log.IsDebugEnabled)
                        _log.DebugFormat("Sent {0} from {1} [{2}]", typeof (T).FullName, Uri, bm.NMSMessageId);

                    session.Commit();
                }
            }
            catch (SerializationException sex)
            {
                throw new MessageException(message.GetType(), "Unable to serialize message", sex);
            }
            catch (Exception ex)
            {
                throw new EndpointException(this, "Unable to send message of type " + typeof (T).FullName, ex);
            }
        }

        public IEnumerable<IMessageSelector> SelectiveReceive(TimeSpan timeout)
        {
            if (_disposed) throw new ObjectDisposedException("The object has been disposed");

            using (var session = _connection.CreateSession(AcknowledgementMode.Transactional))
            {
                var destination = session.GetQueue(_queueName);

                using (var consumer = session.CreateConsumer(destination))
                {
                    IMessage message = consumer.Receive(timeout);

                    using (var selector = new NmsMessageSelector(this, session, message, _serializer))
                    {
                        yield return selector;
                    }
                }
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }


        protected virtual void Dispose(bool disposing)
        {
            if (_disposed) return;
            if (disposing)
            {
                if (_connection != null)
                {
                    _connection.Stop();
                    _connection.ExceptionListener -= ConnectionExceptionListener;
                    _connection.Close();
                    _connection.Dispose();
                    _connection = null;
                }
            }
            _disposed = true;
        }

        private IBytesMessage BuildMessage<T>(ISession session, T message)
        {
            IBytesMessage bm = session.CreateBytesMessage();

            using (MemoryStream mem = new MemoryStream())
            {
                _serializer.Serialize(mem, message);

                bm.Content = mem.ToArray();
            }

            return bm;
        }

        private void ConnectionExceptionListener(Exception ex)
        {
            _log.Error("An exception occurred on the endpoint. Uri = " + _uri, ex);
        }

        ~NmsEndpoint()
        {
            Dispose(false);
        }

        public static IEndpoint ConfigureEndpoint(Uri uri, Action<IEndpointConfigurator> configurator)
        {
            if (uri.Scheme.ToLowerInvariant() == "activemq")
            {
                IEndpoint endpoint = NmsEndpointConfigurator.New(x =>
                    {
                        x.SetUri(uri);

                        configurator(x);
                    });

                return endpoint;
            }

            return null;
        }
    }
}
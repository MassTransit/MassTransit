/// Copyright 2007-2008 The Apache Software Foundation.
/// 
/// Licensed under the Apache License, Version 2.0 (the "License"); you may not use 
/// this file except in compliance with the License. You may obtain a copy of the 
/// License at 
/// 
///   http://www.apache.org/licenses/LICENSE-2.0 
/// 
/// Unless required by applicable law or agreed to in writing, software distributed 
/// under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR 
/// CONDITIONS OF ANY KIND, either express or implied. See the License for the 
/// specific language governing permissions and limitations under the License.
namespace MassTransit.ServiceBus.NMS
{
    using System;
    using System.Runtime.Serialization;
    using System.Threading;
    using Apache.NMS;
    using Apache.NMS.ActiveMQ;
    using Exceptions;
    using Internal;
    using log4net;
    using IMessageConsumer=Apache.NMS.IMessageConsumer;

    public class NmsMessageReceiver :
        IMessageReceiver
    {
        private IEnvelopeConsumer _consumer;
        private readonly IEndpoint _endpoint;
        private IConnectionFactory _factory;
        private readonly string _queueName;
        private IConnection _connection;
        private ISession _session;
        private IDestination _destination;
        private IMessageConsumer _messageConsumer;

        private static readonly ILog _log = LogManager.GetLogger(typeof(NmsMessageSender));
        private static readonly ILog _messageLog = LogManager.GetLogger("MassTransit.Messages");

        public NmsMessageReceiver(IEndpoint endpoint)
        {
            _endpoint = endpoint;

            UriBuilder queueUri = new UriBuilder("tcp", endpoint.Uri.Host, endpoint.Uri.Port);

            _queueName = endpoint.Uri.AbsolutePath.Substring(1);

            _factory = new ConnectionFactory(queueUri.Uri);
        }

        public void Subscribe(IEnvelopeConsumer consumer)
        {
            lock (this)
            {
                if (_consumer == null)
                {
                    _consumer = consumer;

                    Start();
                }
                else if (_consumer != consumer)
                {
                    throw new EndpointException(_endpoint, "Only one consumer can be registered for a message receiver");
                }
            }
        }

        public void Start()
        {
            _connection = _factory.CreateConnection();
            _session = _connection.CreateSession();
            _destination = _session.GetQueue(_queueName);
            _messageConsumer = _session.CreateConsumer(_destination);

            _messageConsumer.Listener += new MessageListener(MessageListener);
        }

        private void MessageListener(IMessage message)
        {
            try
            {
				if (_log.IsDebugEnabled)
					_log.DebugFormat("Queue: {0} Received Message Id {1}", _queueName, message.NMSMessageId);

                if (_consumer != null)
                {
                    try
                    {
                        IEnvelope e = new NmsEnvelopeMapper(_session).ToEnvelope(message);

                        if (_consumer.IsHandled(e))
                        {
                            if (_messageLog.IsInfoEnabled)
                            			_messageLog.InfoFormat("Received message {0} from {1}", e.Messages[0].GetType(), e.ReturnEndpoint.Uri);

                            ThreadPool.QueueUserWorkItem(ProcessMessage, e);
                        }
                    }
                    catch (SerializationException ex)
                    {
                        if(_log.IsErrorEnabled)
                            _log.Error("Discarding unknown message " + message.NMSMessageId, ex);
                        throw;
                    }
                }
            }
            catch (Exception ex)
            {
                if (_log.IsErrorEnabled)
                    _log.Error("Discarding unknown message " + message.NMSMessageId, ex);
                throw;
            }
        }

        public void ProcessMessage(object obj)
        {
            IEnvelope e = obj as IEnvelope;
            if (e == null)
                return;
            try
            {
                _consumer.Deliver(e);

                e.ReturnEndpoint.Dispose();
            }
            catch (Exception ex)
            {
                if (_log.IsErrorEnabled)
                    _log.Error(string.Format("An exception occured delivering envelope {0}", e.Id), ex);
            }
        }

        public void Stop()
        {
            if (_messageConsumer != null)
            {
                _messageConsumer.Dispose();
                _messageConsumer = null;
            }

            _destination = null;

            if (_session != null)
            {
                _session.Dispose();
                _session = null;
            }
            if (_connection != null)
            {
                _connection.Dispose();
                _connection = null;
            }
        }

        public void Dispose()
        {
            Stop();
        }
    }
}
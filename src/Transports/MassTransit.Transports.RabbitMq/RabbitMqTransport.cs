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
namespace MassTransit.Transports.RabbitMq
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using Exceptions;
    using Internal;
    using log4net;
    using RabbitMQ.Client;
    using RabbitMQ.Client.Events;
    using RabbitMQ.Client.Impl;

    [DebuggerDisplay("{Address}")]
    public class RabbitMqTransport :
        ITransport
    {
        static readonly ILog _log = LogManager.GetLogger(typeof(RabbitMqTransport));
        
        readonly IEndpointAddress _address;
        IConnection _connection;
        bool _disposed;
        ConnectionFactory _factory;

        static string _directSendMessageExchange = "";

        public RabbitMqTransport(IEndpointAddress address)
        {
            _address = address;

            Initialize();
        }

        public IEndpointAddress Address
        {
            get { return _address; }
        }

        public void Receive(Func<Stream, Action<Stream>> receiver)
        {
            if (_disposed) throw NewDisposedException();

            Receive(receiver, TimeSpan.Zero);
        }

        public void Receive(Func<Stream, Action<Stream>> receiver, TimeSpan timeout)
        {
            if (_disposed) throw NewDisposedException();

            using(var channel = _connection.CreateModel())
            {
                var result = channel.BasicGet(Address.Uri.PathAndQuery.Replace("/",""), true);

                using(var body = new MemoryStream(result.Body))
                {
                    Action<Stream> receive = receiver(body);
                    if(receive==null)
                    {
                        if (_log.IsDebugEnabled)
                            _log.DebugFormat("SKIP:{0}:{1}", Address, result.BasicProperties.MessageId);

                        if (SpecialLoggers.Messages.IsInfoEnabled)
                            SpecialLoggers.Messages.InfoFormat("SKIP:{0}:{1}", Address, result.BasicProperties.MessageId);
                    }
                    else
                    {
                        receive(body);
                    }
                }

                channel.BasicAck(result.DeliveryTag, false);
                channel.Close(200,"ok");
            }
        }

        public void Send(Action<Stream> sender)
        {
            if (_disposed) throw NewDisposedException();


            using (IModel channel = _connection.CreateModel())
            {
                var properties = channel.CreateBasicProperties();
                SetMessageExpiration(properties);
                using (var bodyStream = new MemoryStream())
                {
                    sender(bodyStream);

                    channel.BasicPublish(_directSendMessageExchange, _address.Path, properties, bodyStream.ToArray());

                }
                channel.Close(200, "ok");

                if (SpecialLoggers.Messages.IsInfoEnabled)
                    SpecialLoggers.Messages.InfoFormat("SEND:{0}:{1}", Address, properties.MessageId);

            }
        }

        private static void SetMessageExpiration(IBasicProperties properties)
        {
            if (OutboundMessage.Headers.ExpirationTime.HasValue)
			{
			    var expirationAsString = (OutboundMessage.Headers.ExpirationTime.Value - DateTime.UtcNow).ToString();
				properties.Expiration = expirationAsString;
			}
        }

        private Uri GenerateServerUri()
        {
            return new UriBuilder("amqp-0-8", Address.Uri.Host, Address.Uri.Port).Uri;
        }
        
        private void Initialize()
        {
            try
            {
                _factory = new ConnectionFactory();

                Connect();
            }
            catch (Exception ex)
            {
                throw new TransportException(Address.Uri, "Failed to initialize transport", ex);
            }
        }

        private void Connect()
        {
            Disconnect();

            _connection = _factory.CreateConnection(GenerateServerUri());
            
            _connection.CallbackException += ConnectionExceptionListener;
        }

        private void Disconnect()
        {
            if (_connection == null) return;

            try
            {
                _connection.CallbackException -= ConnectionExceptionListener;
                _connection.Close();
                _connection.Dispose();
                _connection = null;
            }
            catch (Exception ex)
            {
                _log.Error("Failed to close existing connection", ex);
            }
        }

        private void Reconnect()
        {
            try
            {
                Disconnect();
            }
            catch (Exception ex)
            {
                _log.Error("Failed to disconnect from " + Address, ex);
            }
            finally
            {
                Connect();
            }
        }

        private void ConnectionExceptionListener(object sender, CallbackExceptionEventArgs ex)
        {
            _log.Error("An exception occurred on the endpoint: " + Address, ex.Exception);

            Reconnect();
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
                //Disconnect();
            }

            _disposed = true;
        }
        
        private ObjectDisposedException NewDisposedException()
        {
            return new ObjectDisposedException("The transport has already been disposed: " + Address);
        }

        ~RabbitMqTransport()
		{
			Dispose(false);
		}
    }
}
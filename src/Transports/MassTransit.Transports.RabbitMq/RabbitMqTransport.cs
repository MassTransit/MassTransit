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
    using System.IO;
    using Internal;
    using log4net;
    using RabbitMQ.Client;

    public class RabbitMqTransport :
        TransportBase
    {
        static readonly ILog _log = LogManager.GetLogger(typeof(RabbitMqTransport));

        readonly IConnection _connection;

        static string _directSendMessageExchange = "";
        RabbitMqAddress _address;

        public RabbitMqTransport(IEndpointAddress address, IConnection connection) : base(address)
        {
            _connection = connection;
            _address = new RabbitMqAddress(address.Uri);
        }

        public override void Receive(Func<Stream, Action<Stream>> receiver, TimeSpan timeout)
        {
            EnsureNotDisposed();

            using(var channel = _connection.CreateModel())
            {
                channel.QueueDeclare(_address.Queue, true);
                var result = channel.BasicGet(_address.Queue, true);
                
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

        public override void Send(Action<ISendingContext> sender)
        {
            EnsureNotDisposed();

            using (var channel = _connection.CreateModel())
            {
                var properties = channel.CreateBasicProperties();
                SetMessageExpiration(properties);
                using (var bodyStream = new MemoryStream())
                {
                    var cxt = new RabbitMqSendingContext();
                    cxt.Body = bodyStream;
                    sender(cxt);
                    channel.ExchangeDeclare(_address.Queue, "fanout", true);
                    channel.BasicPublish(_address.Queue, "msg", properties, bodyStream.ToArray());

                }
                channel.Close(200, "ok");

                if (SpecialLoggers.Messages.IsInfoEnabled)
                    SpecialLoggers.Messages.InfoFormat("SEND:{0}:{1}", Address, properties.MessageId);

            }
        }

        public override void Send(Action<Stream> sender)
        {
            EnsureNotDisposed();

            using (var channel = _connection.CreateModel())
            {
                var properties = channel.CreateBasicProperties();
                SetMessageExpiration(properties);
                using (var bodyStream = new MemoryStream())
                {
                    sender(bodyStream);
                    channel.ExchangeDeclare(_address.Queue, "fanout", true);
                    channel.BasicPublish(_address.Queue, "msg", properties, bodyStream.ToArray());

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

        public override void OnDisposing()
        {
            //no-op
        }
    }

    public class RabbitMqSendingContext :
        ISendingContext
    {
        public Stream Body
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        public void MarkRecoverable()
        {
            throw new NotImplementedException();
        }

        public void SetLabel(string label)
        {
            throw new NotImplementedException();
        }

        public void SetMessageExpiration(DateTime d)
        {
            throw new NotImplementedException();
        }
    }
}
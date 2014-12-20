// Copyright 2007-2012 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
namespace MassTransit.RabbitMqTransport
{
    using System;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using Magnum;
    using RabbitMQ.Client;
    using RabbitMQ.Client.Exceptions;
    using Transports;


    public class OutboundRabbitMqTransport 
    {
        readonly IRabbitMqEndpointAddress _address;
        readonly bool _bindToQueue;
        RabbitMqProducer _producer;

        public OutboundRabbitMqTransport(IRabbitMqEndpointAddress address,
             bool bindToQueue)
        {
            _address = address;
            _bindToQueue = bindToQueue;
        }

        public EndpointAddress Address
        {
            get { return new EndpointAddress(_address.Uri); }
        }

        public void Send(ISendContext context)
        {
            AddProducerBinding();

                {
                    try
                    {
//                        IBasicProperties properties = _producer.CreateProperties();
//
//                        properties.SetPersistent(context.DeliveryMode == DeliveryMode.Persistent);
//                        properties.MessageId = context.MessageId ?? properties.MessageId ?? NewId.Next().ToString();
//                        if (context.ExpirationTime.HasValue)
//                        {
//                            DateTime value = context.ExpirationTime.Value;
//                            properties.Expiration =
//                                (value.Kind == DateTimeKind.Utc
//                                     ? value - SystemUtil.UtcNow
//                                     : value - SystemUtil.Now).
//                                    TotalMilliseconds.ToString("F0", CultureInfo.InvariantCulture);
//                        }
//
//                        using (var body = new MemoryStream())
//                        {
//                            context.SerializeTo(body);
//                            properties.Headers = context.Headers.ToDictionary(entry => entry.Key, entry => (object)entry.Value);
//                            properties.Headers["Content-Type"]=context.ContentType;
//
//                            var task = _producer.PublishAsync(_address.Name, properties, body.ToArray());
//                            task.Wait();
//
//                            _address.Uri.LogSent(context.MessageId ?? properties.MessageId ?? "", context.MessageType);
//                        }
                    }
                    catch (AggregateException ex)
                    {
                        throw new TransportException(_address.Uri, "Publisher did not confirm message", ex.InnerException);
                    }
                    catch (AlreadyClosedException ex)
                    {
                        throw new InvalidConnectionException(_address.Uri, "Connection was already closed", ex);
                    }
                    catch (EndOfStreamException ex)
                    {
                        throw new InvalidConnectionException(_address.Uri, "Connection was closed", ex);
                    }
                    catch (OperationInterruptedException ex)
                    {
                        throw new InvalidConnectionException(_address.Uri, "Operation was interrupted", ex);
                    }
                }
        }

        public void Dispose()
        {
            RemoveProducer();
        }

        void AddProducerBinding()
        {
            if (_producer != null)
                return;

            _producer = new RabbitMqProducer(_address, _bindToQueue);

        }

        void RemoveProducer()
        {
            if (_producer != null)
            {
            }
        }
    }
}
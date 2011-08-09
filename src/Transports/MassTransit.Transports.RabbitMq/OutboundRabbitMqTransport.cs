// Copyright 2007-2011 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
	using System.Collections;
	using System.IO;
	using Magnum;
	using RabbitMQ.Client;
	using RabbitMQ.Client.Exceptions;

	public class OutboundRabbitMqTransport :
		IOutboundTransport
	{
		readonly IRabbitMqEndpointAddress _address;
		readonly ConnectionHandler<RabbitMqConnection> _connectionHandler;
		RabbitMqProducer _producer;
		bool _bindToQueue;

		public OutboundRabbitMqTransport(IRabbitMqEndpointAddress address,
		                                 ConnectionHandler<RabbitMqConnection> connectionHandler, bool bindToQueue)
		{
			_address = address;
			_connectionHandler = connectionHandler;
			_bindToQueue = bindToQueue;
		}

		public IEndpointAddress Address
		{
			get { return _address; }
		}

		public void Send(ISendContext context)
		{
			AddProducerBinding();

			_connectionHandler.Use(connection =>
				{
					try
					{
						IBasicProperties properties = _producer.Channel.CreateBasicProperties();

						properties.SetPersistent(true);
						if (context.ExpirationTime.HasValue)
						{
							DateTime value = context.ExpirationTime.Value;
							properties.Expiration =
								(value.Kind == DateTimeKind.Utc ? value - SystemUtil.UtcNow : value - SystemUtil.Now).ToString();
						}

						using (var body = new MemoryStream())
						{
							context.SerializeTo(body);
							properties.Headers = new Hashtable {{"Content-Type", context.ContentType}};

							_producer.Channel.BasicPublish(_address.Name, "", properties, body.ToArray());
						}
					}
					catch (EndOfStreamException ex)
					{
						throw new InvalidConnectionException(_address.Uri, "Connection was closed", ex);
					}
					catch (OperationInterruptedException ex)
					{
						throw new InvalidConnectionException(_address.Uri, "Operation was interrupted", ex);
					}
				});
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

			_connectionHandler.AddBinding(_producer);
		}

		void RemoveProducer()
		{
			if (_producer != null)
			{
				_connectionHandler.RemoveBinding(_producer);
			}
		}
	}
}
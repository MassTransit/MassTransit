// Copyright 2007-2011 The Apache Software Foundation.
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
	using log4net;
	using RabbitMQ.Client;
	using Util;

	public class RabbitMqTransport :
		TransportBase
	{
		private static readonly ILog _log = LogManager.GetLogger(typeof (RabbitMqTransport));

		private static string _directSendMessageExchange = "";
		private readonly IConnection _connection;
		private RabbitMqAddress _address;

		public RabbitMqTransport(IEndpointAddress address, IConnection connection)
			: base(address)
		{
			_connection = connection;
			_address = new RabbitMqAddress(address.Uri);
		}


		public override void Receive(Func<IReceiveContext, Action<IReceiveContext>> callback, TimeSpan timeout)
		{
			GuardAgainstDisposed();

			using (IModel channel = _connection.CreateModel())
			{
				channel.QueueDeclare(_address.Queue, true);
				BasicGetResult result = channel.BasicGet(_address.Queue, true);

				using (var context = new RabbitMqReceiveContext(result))
				{
					Action<IReceiveContext> receive = callback(context);
					if (receive == null)
					{
						if (_log.IsDebugEnabled)
							_log.DebugFormat("SKIP:{0}:{1}", Address, result.BasicProperties.MessageId);

						if (SpecialLoggers.Messages.IsInfoEnabled)
							SpecialLoggers.Messages.InfoFormat("SKIP:{0}:{1}", Address, result.BasicProperties.MessageId);
					}
					else
					{
						receive(context);
					}
				}

				channel.BasicAck(result.DeliveryTag, false);
				channel.Close(200, "ok");
			}
		}

		public override void Send(Action<ISendContext> callback)
		{
			GuardAgainstDisposed();

			using (IModel channel = _connection.CreateModel())
			{
				channel.ExchangeDeclare(_address.Queue, "fanout", true);

				using (var context = new RabbitMqSendContext(channel))
				{
					callback(context);

					channel.BasicPublish(_address.Queue, "msg", context.Properties, context.GetBytes());

					if (SpecialLoggers.Messages.IsInfoEnabled)
						SpecialLoggers.Messages.InfoFormat("SEND:{0}:{1}", Address, context.Properties.MessageId);
				}

				channel.Close(200, "ok");
			}
		}


		public override void OnDisposing()
		{
			//no-op
		}
	}
}
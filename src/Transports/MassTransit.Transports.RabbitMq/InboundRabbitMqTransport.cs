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
	using log4net;
	using RabbitMQ.Client;
	using Util;

	public class InboundRabbitMqTransport : 
		IInboundTransport
	{
		static readonly ILog _log = LogManager.GetLogger(typeof (InboundRabbitMqTransport));

		readonly RabbitMqAddress _address;
		readonly IConnection _connection;

		public InboundRabbitMqTransport(RabbitMqAddress address, IConnection connection)
		{
			_address = address;
			_connection = connection;
		}

		public IEndpointAddress Address { get; private set; }

		public void Receive(Func<IReceiveContext, Action<IReceiveContext>> callback, TimeSpan timeout)
		{
			//GuardAgainstDisposed();

			using (IModel channel = _connection.CreateModel())
			{
				channel.QueueDeclare(_address.Path, true, true, true, null);
                BasicGetResult result = channel.BasicGet(_address.Path, false); //this is odd but false turns ack on.

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

		public void Dispose()
		{
			_connection.Dispose();
		}
	}
}
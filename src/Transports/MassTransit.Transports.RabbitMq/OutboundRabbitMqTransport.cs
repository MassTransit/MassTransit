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
	using RabbitMQ.Client;
	using Util;

	public class OutboundRabbitMqTransport :
		IOutboundTransport
	{
		readonly IRabbitMqEndpointAddress _address;
		readonly IConnection _connection;
		bool _declared;

		public OutboundRabbitMqTransport(IRabbitMqEndpointAddress address, IConnection connection)
		{
			_address = address;
			_connection = connection;
		}

		public IEndpointAddress Address
		{
			get { return _address; }
		}

		public void Send(Action<ISendContext> callback)
		{
			//GuardAgainstDisposed();

			using (IModel channel = _connection.CreateModel())
			{
				DeclareBindings(channel);

				using (var context = new RabbitMqSendContext(channel))
				{
					callback(context);

					channel.BasicPublish(_address.Name, "", context.Properties, context.GetBytes());

					if (SpecialLoggers.Messages.IsInfoEnabled)
						SpecialLoggers.Messages.InfoFormat("SEND:{0}:{1}", Address, context.Properties.MessageId);
				}

				channel.Close(200, "ok");
			}
		}

		void DeclareBindings(IModel channel)
		{
			if (_declared)
				return;

			channel.ExchangeDeclare(_address.Name, ExchangeType.Fanout, true);

			_declared = true;
		}

		public void Dispose()
		{
			_connection.Dispose();
		}
	}
}
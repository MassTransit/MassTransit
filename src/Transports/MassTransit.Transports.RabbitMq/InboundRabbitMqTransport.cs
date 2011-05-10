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
	using System.Threading;
	using log4net;
	using Magnum;
	using Management;
	using RabbitMQ.Client;
	using Util;

	public class InboundRabbitMqTransport :
		IInboundTransport
	{
		static readonly ILog _log = LogManager.GetLogger(typeof (InboundRabbitMqTransport));

		readonly IRabbitMqEndpointAddress _address;
		readonly IConnection _connection;
		bool _declared;

		public InboundRabbitMqTransport(IRabbitMqEndpointAddress address, IConnection connection)
		{
			_address = address;
			_connection = connection;
		}

		public IEndpointAddress Address
		{
			get { return _address; }
		}

		public void Receive(Func<IReceiveContext, Action<IReceiveContext>> callback, TimeSpan timeout)
		{
			DeclareBindings();

			//GuardAgainstDisposed();

			using (IModel channel = _connection.CreateModel())
			{
				BasicGetResult result = channel.BasicGet(_address.Name, false); //this is odd but false turns ack on.
				if (result == null)
				{
					ThreadUtil.Sleep(timeout);
					return;				
				}

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

		void DeclareBindings()
		{
			if (_declared)
				return;

			using (var management = new RabbitMqEndpointManagement(_address, _connection))
			{
				management.BindQueue(_address.Name, _address.Name, ExchangeType.Fanout, "");
			}

			_declared = true;
		}
	}
}
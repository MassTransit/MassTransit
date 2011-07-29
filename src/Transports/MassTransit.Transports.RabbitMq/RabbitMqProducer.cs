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
	using Management;
	using RabbitMQ.Client;

	public class RabbitMqProducer :
		ConnectionBinding<RabbitMqConnection>
	{
		readonly IRabbitMqEndpointAddress _address;
		IModel _channel;
		bool _bindToQueue;

		public RabbitMqProducer(IRabbitMqEndpointAddress address, bool bindToQueue)
		{
			_address = address;
			_bindToQueue = bindToQueue;
		}

		public IModel Channel
		{
			get { return _channel; }
		}

		public void Bind(RabbitMqConnection connection)
		{
			_channel = connection.Connection.CreateModel();

			_channel.ExchangeDeclare(_address.Name, ExchangeType.Fanout, true);

			if(_bindToQueue)
			{
				using (var management = new RabbitMqEndpointManagement(_address, connection.Connection))
				{
					management.BindQueue(_address.Name, _address.Name, ExchangeType.Fanout, "");
				}
			}
		}

		public void Unbind(RabbitMqConnection connection)
		{
			if (_channel != null)
			{
				if (_channel.IsOpen)
					_channel.Close(200, "producer unbind");
				_channel.Dispose();
				_channel = null;
			}
		}
	}
}
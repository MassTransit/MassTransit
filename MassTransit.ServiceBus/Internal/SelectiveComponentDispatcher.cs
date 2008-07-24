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
namespace MassTransit.ServiceBus.Internal
{
	using System.Collections;

	public class SelectiveComponentDispatcher<TComponent, TMessage> :
		Consumes<TMessage>.Selected
		where TMessage : class
		where TComponent : class
	{
		private readonly IObjectBuilder _builder;
		private readonly IServiceBus _bus;

		public SelectiveComponentDispatcher(IObjectBuilder builder, IServiceBus bus)
		{
			_builder = builder;
			_bus = bus;
		}

		public bool Accept(TMessage message)
		{
			TComponent component = _builder.Build<TComponent>();

			try
			{
				Consumes<TMessage>.Selected consumer = component as Consumes<TMessage>.Selected;

				if (consumer != null)
					return consumer.Accept(message);

				return false;
			}
			finally
			{
				_builder.Release(component);
			}
		}

		public void Consume(TMessage message)
		{
			TComponent component;
			if (_bus != null)
			{
				Hashtable arguments = _bus.GetProducers<TComponent>();
				component = _builder.Build<TComponent>(arguments);
			}
			else
			{
				component = _builder.Build<TComponent>();
			}

			try
			{
				if(_bus != null)
					_bus.AttachProducers(component);

				Consumes<TMessage>.Selected consumer = component as Consumes<TMessage>.Selected;
				if (consumer != null)
					consumer.Consume(message);
			}
			finally
			{
				_builder.Release(component);
			}
		}
	}
}
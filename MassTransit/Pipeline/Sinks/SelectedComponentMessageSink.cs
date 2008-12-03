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
namespace MassTransit.Pipeline.Sinks
{
	using System.Collections.Generic;
	using Interceptors.Inbound;

	/// <summary>
	/// Routes messages to instances of subscribed components. A new instance of the component
	/// is created from the container for each message received.
	/// </summary>
	/// <typeparam name="TComponent">The component type to handle the message</typeparam>
	/// <typeparam name="TMessage">The message to handle</typeparam>
	public class SelectedComponentMessageSink<TComponent, TMessage> :
		IMessageSink<TMessage>
		where TMessage : class
		where TComponent : class, Consumes<TMessage>.Selected
	{
		private readonly IObjectBuilder _builder;

		public SelectedComponentMessageSink(IInboundContext context)
		{
			_builder = context.Builder;
		}

		public void Dispose()
		{
		}

		public IEnumerable<Consumes<TMessage>.All> Enumerate(TMessage message)
		{
			Consumes<TMessage>.Selected consumer = BuildConsumer();

			try
			{
				if (consumer.Accept(message) == false)
					yield break;

				yield return consumer;
			}
			finally
			{
				Release(consumer);
			}
		}

		public bool Inspect(IPipelineInspector inspector)
		{
			return inspector.Inspect(this);
		}

		private void Release(Consumes<TMessage>.Selected consumer)
		{
			_builder.Release(TranslateTo<TComponent>.From(consumer));
		}

		private Consumes<TMessage>.Selected BuildConsumer()
		{
			TComponent component = _builder.GetInstance<TComponent>();

			Consumes<TMessage>.Selected consumer = TranslateTo<Consumes<TMessage>.Selected>.From(component);

			return consumer;
		}
	}
}
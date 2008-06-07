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
	using System;

	public class GenericComponent<TMessage> :
		Consumes<TMessage>.Selected, IEquatable<GenericComponent<TMessage>> where TMessage : class
	{
		private readonly IServiceBus _bus;
		private readonly Action<IMessageContext<TMessage>> _wrappedAction;
		private readonly Predicate<TMessage> _wrappedCondition;

		public GenericComponent(Action<IMessageContext<TMessage>> wrappedAction, IServiceBus bus)
		{
			_wrappedAction = wrappedAction;
			_wrappedCondition = null;
			_bus = bus;
		}

		public GenericComponent(Action<IMessageContext<TMessage>> wrappedAction, Predicate<TMessage> condition, IServiceBus bus)
		{
			_wrappedAction = wrappedAction;
			_wrappedCondition = condition;
			_bus = bus;
		}

		public void Consume(TMessage message)
		{
			IEnvelope notSureHowToGet = null;
			IMessageContext<TMessage> cxt = new MessageContext<TMessage>(_bus, notSureHowToGet, message);
			_wrappedAction(cxt);
		}

		public bool Accept(TMessage message)
		{
			if (_wrappedCondition == null)
				return true;

			return _wrappedCondition(message);
		}

		public bool Equals(GenericComponent<TMessage> genericComponent)
		{
			if (genericComponent == null) return false;
			return Equals(_wrappedAction, genericComponent._wrappedAction) && Equals(_wrappedCondition, genericComponent._wrappedCondition);
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(this, obj)) return true;
			return Equals(obj as GenericComponent<TMessage>);
		}

		public override int GetHashCode()
		{
			return (_wrappedAction != null ? _wrappedAction.GetHashCode() : 0) + 29*(_wrappedCondition != null ? _wrappedCondition.GetHashCode() : 0);
		}
	}
}
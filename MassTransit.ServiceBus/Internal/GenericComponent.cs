namespace MassTransit.ServiceBus.Internal
{
	using System;

	public class GenericComponent<TMessage> :
		Consumes<TMessage>.Selected, IEquatable<GenericComponent<TMessage>> where TMessage : class, IMessage
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
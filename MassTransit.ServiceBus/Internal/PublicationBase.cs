namespace MassTransit.Internal
{
	using System;
	using System.Collections.Generic;
	using Subscriptions;

	public abstract class PublicationBase<TMessage> : 
		IPublicationTypeInfo 
		where TMessage : class
	{
		protected Type _messageType;
		protected TimeSpan _timeToLive;

		protected PublicationBase()
		{
			_messageType = typeof (TMessage);
			_timeToLive = GetMessageTimeToLive();
		}

		public TimeSpan TimeToLive
		{
			get { return _timeToLive; }
		}

		private TimeSpan GetMessageTimeToLive()
		{
			TimeSpan timeToLive = TimeSpan.MaxValue;

			object[] attributes = _messageType.GetCustomAttributes(typeof(ExpiresInAttribute), false);
			foreach (ExpiresInAttribute attribute in attributes)
			{
				if (attribute.TimeSpan < _timeToLive)
					timeToLive = attribute.TimeSpan;
			}

			return timeToLive;
		}

		public abstract IList<Subscription> GetConsumers<T>(IDispatcherContext context, T message) where T : class;

		public abstract void PublishFault<T>(IServiceBus bus, Exception ex, T message) where T : class;
	}
}
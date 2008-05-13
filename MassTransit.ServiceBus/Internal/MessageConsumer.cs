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
	using System.Collections.Generic;
	using Exceptions;
	using log4net;
	using Threading;

	/// <summary>
	/// A message consumer is created when a service subscribes to a specific type of message
	/// on a service bus. 
	/// </summary>
	/// <typeparam name="T">The message type handled by this message consumer</typeparam>
	public class MessageConsumer<T> :
		IMessageConsumer<T> where T : IMessage
	{
		private static readonly ILog _log = LogManager.GetLogger(typeof (MessageConsumer<T>));

		private readonly List<MessageConsumerCallbackItem<T>> _callbacks = new List<MessageConsumerCallbackItem<T>>();

		#region IMessageConsumer<T> Members

		/// <summary>
		/// Adds a subscription to the message type for the specified handler
		/// </summary>
		/// <param name="callback">The function to call to handle the message</param>
		/// <param name="condition">The condition function to determine if a message will be handled</param>
		public void Subscribe(Action<IMessageContext<T>> callback, Predicate<T> condition)
		{
			_callbacks.Add(new MessageConsumerCallbackItem<T>(callback, condition));
		}

		public void Unsubscribe(Action<IMessageContext<T>> callback, Predicate<T> condition)
		{
			MessageConsumerCallbackItem<T> match = new MessageConsumerCallbackItem<T>(callback, condition);

			if (_callbacks.Contains(match))
				_callbacks.Remove(match);
		}

		public int Count
		{
			get { return _callbacks.Count; }
		}

		/// <summary>
		/// Deliver the message to the handler
		/// </summary>
		/// <param name="bus">The service bus where the message arrived</param>
		/// <param name="envelope">The envelope containing the message</param>
		/// <param name="message">The message being delivered</param>
		public void Deliver(IServiceBus bus, IEnvelope envelope, T message)
		{
			MessageContext<T> context = new MessageContext<T>(bus, envelope, message);

			foreach (MessageConsumerCallbackItem<T> item in _callbacks)
			{
				if (item.Condition != null)
				{
					if (item.Condition(context.Message) == false)
						break;
				}

				item.Callback(context);
			}
		}

		/// <summary>
		/// Deliver the message to the handler
		/// </summary>
		/// <param name="bus">The service bus where the message arrived</param>
		/// <param name="envelope">The envelope containing the message</param>
		/// <param name="message">The message being delivered</param>
		public void Deliver(IServiceBus bus, IEnvelope envelope, IMessage message)
		{
			Deliver(bus, envelope, (T) message);
		}

		/// <summary>
		/// Allows the handler to determine if it will handle the message before retrieving it
		/// </summary>
		/// <param name="message">The message to handle</param>
		/// <returns>True if the message will be handled, otherwise false.</returns>
		public bool IsHandled(T message)
		{
			foreach (MessageConsumerCallbackItem<T> item in _callbacks)
			{
				if (item.Condition == null)
					return true;

				try
				{
					if (item.Condition(message))
						return true;
				}
				catch (Exception ex)
				{
					throw new MessageConsumerException<T>(item,
					                                      "There was an exception in the MessageConsumer.IsHandled",
					                                      ex);
				}
			}

			return false;
		}

		/// <summary>
		/// Allows the handler to determine if it will handle the message before retrieving it
		/// </summary>
		/// <param name="message">The message to handle</param>
		/// <returns>True if the message will be handled, otherwise false.</returns>
		public bool IsHandled(IMessage message)
		{
			return IsHandled((T) message);
		}

		#endregion

		private void Dispatch(MessageContext<T> obj)
		{
			throw new NotImplementedException();
		}
	}

	/// <summary>
	/// Used to track a subscription to a message type on a service bus
	/// </summary>
	/// <typeparam name="T1">The type of message being handled</typeparam>
	public class MessageConsumerCallbackItem<T1> : IEquatable<MessageConsumerCallbackItem<T1>> where T1 : IMessage

	{
		private Action<IMessageContext<T1>> _callback;
		private Predicate<T1> _condition;

		/// <summary>
		/// Initializes an instance of a <c ref="MessageConsumerCallbackItem" />
		/// </summary>
		/// <param name="callback">The callback method to handle the message</param>
		public MessageConsumerCallbackItem(Action<IMessageContext<T1>> callback)
		{
			_callback = callback;
		}

		/// <summary>
		/// Initializes an instance of a <c ref="MessageConsumerCallbackItem" />
		/// </summary>
		/// <param name="callback">The callback method to handle the message</param>
		/// <param name="condition">The predicate used to check if the callback will handle the message</param>
		public MessageConsumerCallbackItem(Action<IMessageContext<T1>> callback, Predicate<T1> condition)
		{
			_callback = callback;
			_condition = condition;
		}

		/// <summary>
		/// The callback method
		/// </summary>
		public Action<IMessageContext<T1>> Callback
		{
			get { return _callback; }
			set { _callback = value; }
		}

		/// <summary>
		/// The message filter condition
		/// </summary>
		public Predicate<T1> Condition
		{
			get { return _condition; }
			set { _condition = value; }
		}

		#region IEquatable<MessageConsumerCallbackItem<T1>> Members

		public bool Equals(MessageConsumerCallbackItem<T1> other)
		{
			if (other == null)
				return false;
			if (other.Callback != _callback)
				return false;
			if (other.Condition != _condition)
				return false;

			return true;
		}

		#endregion

		public override bool Equals(object obj)
		{
			MessageConsumerCallbackItem<T1> other = obj as MessageConsumerCallbackItem<T1>;
			if (other == null)
				return false;

			return Equals(other);
		}

		public override int GetHashCode()
		{
			if (_condition == null)
				return _callback.GetHashCode();

			return _callback.GetHashCode() + 29*_condition.GetHashCode();
		}
	}
}
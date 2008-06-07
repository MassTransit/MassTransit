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

	public class MessageDispatcher<TMessage> : IMessageDispatcher<TMessage> where TMessage : class
	{
		private readonly List<Consumes<TMessage>.All> _consumers = new List<Consumes<TMessage>.All>();

		public bool Accept(TMessage message)
		{
			IList<Consumes<TMessage>.All> consumers = new List<Consumes<TMessage>.All>(_consumers);

			foreach (Consumes<TMessage>.All consumer in consumers)
			{
				Consumes<TMessage>.Selected selectiveConsumer = consumer as Consumes<TMessage>.Selected;
				if (selectiveConsumer != null)
				{
					// if the consumer is selective, ask if they want it and if so return true
					if (selectiveConsumer.Accept(message))
						return true;
				}
				else
				{
					// they aren't selective, so return true
					return true;
				}
			}

			return false;
		}

		public void Consume(TMessage message)
		{
			IList<Consumes<TMessage>.All> consumers = new List<Consumes<TMessage>.All>(_consumers);

			foreach (Consumes<TMessage>.All consumer in consumers)
			{
				Consumes<TMessage>.Selected selectiveConsumer = consumer as Consumes<TMessage>.Selected;
				if (selectiveConsumer != null)
				{
					if (selectiveConsumer.Accept(message))
					{
						consumer.Consume(message);
					}
				}
				else
				{
					consumer.Consume(message);
				}
			}
		}

		public bool Accept(object obj)
		{
			TMessage message = obj as TMessage;
			if (message == null)
				throw new ArgumentException("The message is not of type " + typeof (TMessage).FullName, "obj");

			return Accept(message);
		}

		public void Consume(object obj)
		{
			TMessage message = obj as TMessage;
			if (message == null)
				throw new ArgumentException("The message is not of type " + typeof (TMessage).FullName, "obj");

			Consume(message);
		}

		public bool Active
		{
			get { return _consumers.Count > 0; }
		}

		public void Dispose()
		{
			_consumers.Clear();
		}

		public void Attach(Consumes<TMessage>.All consumer)
		{
			lock (_consumers)
			{
				if (!_consumers.Contains(consumer))
				{
					_consumers.Add(consumer);
				}
			}
		}

		public void Detach(Consumes<TMessage>.All consumer)
		{
			lock (_consumers)
			{
				if (_consumers.Contains(consumer))
				{
					_consumers.Remove(consumer);
				}
			}
		}
	}
}
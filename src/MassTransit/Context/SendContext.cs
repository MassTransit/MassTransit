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
namespace MassTransit.Context
{
	using System;
	using System.IO;

	public class SendContext<T> :
		MessageContext,
		ISendContext<T>
		where T : class
	{
		readonly T _message;
		Action<Stream> _bodyWriter;

		public SendContext(T message)
		{
			_message = message;

			this.SetMessageType(typeof (T));
			DeclaringMessageType = typeof (T);
		}

		protected SendContext(T message, IMessageContext context)
		{
			_message = message;
			SetUsing(context);

			this.SetMessageType(typeof (T));
		}

		public Type DeclaringMessageType { get; private set; }

		public void SerializeTo(Stream stream)
		{
			if (_bodyWriter == null)
				throw new InvalidOperationException("No message body writer was specified");

			_bodyWriter(stream);
		}

		public virtual bool TryGetContext<TMessage>(out IBusPublishContext<TMessage> context)
			where TMessage : class
		{
			context = null;

			if (typeof (TMessage).IsAssignableFrom(typeof (T)))
			{
				context = new PublishContext<TMessage>(_message as TMessage, this);
			}

			return context != null;
		}

		public T Message
		{
			get { return _message; }
		}

		public void SetBodyWriter(Action<Stream> bodyWriter)
		{
			_bodyWriter = bodyWriter;
		}
	}
}
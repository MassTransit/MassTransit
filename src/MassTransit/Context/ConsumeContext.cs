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
	using Magnum.Reflection;
	using Serialization;

	public class ConsumeContext :
		MessageContext,
		IBusContext
	{
		readonly Stream _bodyStream;

		object _message;
		IMessageSerializer _serializer;

		public ConsumeContext(Stream bodyStream)
		{
			_bodyStream = bodyStream;
		}

		/// <summary>
		/// The endpoint from which the message was received
		/// </summary>
		public IEndpoint Endpoint { get; private set; }

		object Message
		{
			get
			{
				if (_message == null)
				{
					_bodyStream.Seek(0, SeekOrigin.Begin);
					_message = _serializer.Deserialize(this);
				}

				return _message;
			}
		}


		/// <summary>
		/// The bus on which the message was received
		/// </summary>
		public IServiceBus Bus { get; private set; }


		public void SetBus(IServiceBus bus)
		{
			Bus = bus;
		}

		public void SetEndpoint(IEndpoint endpoint)
		{
			Endpoint = endpoint;
		}

		public void CopyBodyTo(Stream stream)
		{
			_bodyStream.Seek(0, SeekOrigin.Begin);
			_bodyStream.CopyTo(stream);
		}

		public Stream BodyStream
		{
			get
			{
				_bodyStream.Seek(0, SeekOrigin.Begin);
				return _bodyStream;
			}
		}

		public void SetSerializer(IMessageSerializer serializer)
		{
			_serializer = serializer;
		}

		public bool TryGetContext<T>(out IConsumeContext<T> context)
			where T : class
		{
			if (typeof (T).IsAssignableFrom(Message.GetType()))
			{
				context = new ConsumeContext<T>(this, Message as T);
				return true;
			}

			context = null;
			return false;
		}

		/// <summary>
		/// Puts the message back on the queue to be retried later
		/// </summary>
		public void RetryLater()
		{
			if (Message == null)
				throw new InvalidOperationException("RetryLater can only be called when a message is being consumed");

			this.FastInvoke(x => x.RetryLater(Message), Message);
		}

		/// <summary>
		/// Respond to the current inbound message with either a send to the ResponseAddress or a
		/// Publish on the bus that received the message
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="message">The message to send/publish</param>
		/// <param name="contextCallback">The action to setup the context on the outbound message</param>
		public void Respond<T>(T message, Action<ISendContext<T>> contextCallback) where T : class
		{
			if (ResponseAddress != null)
			{
				GetResponseEndpoint().Send(message, context =>
					{
						context.SetSourceAddress(Bus.Endpoint.Uri);
						contextCallback(context);
					});
			}
			else
			{
				Bus.Publish(message, contextCallback);
			}
		}

		/// <summary>
		/// Send a fault to either via publishing or to the Fault Endpoint
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="message">The message to send/publish</param>
		public void GenerateFault<T>(T message)
			where T : class
		{
			if (ResponseAddress != null)
			{
				GetFaultEndpoint().Send(message, context => context.SetSourceAddress(Bus.Endpoint.Uri));
			}
			else
			{
				Bus.Publish(message);
			}
		}

		void RetryLater<T>(T message)
			where T : class
		{
			if (Message == null)
				throw new InvalidOperationException("No message is available to retry later");

			Bus.Endpoint.Send(message, x =>
				{
					x.SetUsing(this);
					x.SetRetryCount(RetryCount + 1);
				});
		}

		IEndpoint GetFaultEndpoint()
		{
			if (FaultAddress == null)
				throw new InvalidOperationException("No fault address was contained in the message");

			return Bus.GetEndpoint(FaultAddress);
		}

		IEndpoint GetResponseEndpoint()
		{
			if (ResponseAddress == null)
				throw new InvalidOperationException("No response address was contained in the message");

			return Bus.GetEndpoint(ResponseAddress);
		}
	}

	public class ConsumeContext<TMessage> :
		IConsumeContext<TMessage>
	{
		readonly IConsumeContext _context;
		readonly TMessage _message;
		Uri _responseAddress;

		public ConsumeContext(IConsumeContext context, TMessage message)
		{
			_context = context;
			_message = message;
			_responseAddress = context.ResponseAddress;
		}

		public string MessageId
		{
			get { return _context.MessageId; }
		}

		public string MessageType
		{
			get { return typeof (TMessage).ToMessageName(); }
		}

		public Uri SourceAddress
		{
			get { return _context.SourceAddress; }
		}

		public Uri DestinationAddress
		{
			get { return _context.DestinationAddress; }
		}

		public Uri ResponseAddress
		{
			get { return _responseAddress; }
		}

		public Uri FaultAddress
		{
			get { return _context.FaultAddress; }
		}

		public string Network
		{
			get { return _context.MessageId; }
		}

		public DateTime? ExpirationTime
		{
			get { return _context.ExpirationTime; }
		}

		public int RetryCount
		{
			get { return _context.RetryCount; }
		}

		public IServiceBus Bus
		{
			get { return _context.Bus; }
		}

		public TMessage Message
		{
			get { return _message; }
		}

		public bool TryGetContext<T>(out IConsumeContext<T> context)
			where T : class
		{
			if (typeof (T).IsAssignableFrom(Message.GetType()))
			{
				context = new ConsumeContext<T>(this, Message as T);
				return true;
			}

			context = null;
			return false;
		}

		public void RetryLater()
		{
			_context.RetryLater();
		}

		public void Respond<T>(T message, Action<ISendContext<T>> contextCallback)
			where T : class
		{
			_context.Respond(message, contextCallback);
		}

		public void GenerateFault<T>(T message)
			where T : class
		{
			_context.GenerateFault(message);
		}

		public void SetResponseAddress(Uri value)
		{
			_responseAddress = value;
		}
	}
}
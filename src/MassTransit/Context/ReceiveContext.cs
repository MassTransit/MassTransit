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
	using System.Collections.Generic;
	using System.Diagnostics;
	using System.IO;
	using System.Runtime.Serialization;
	using log4net;
	using Magnum;
	using Magnum.Extensions;
	using Magnum.Reflection;
	using Serialization;
	using Util;

	public class ReceiveContext :
		MessageContext,
		IReceiveContext
	{
		Stream _bodyStream;
		IMessageTypeConverter _typeConverter;
		readonly IList<ISent> _sent;
		readonly IList<IPublished> _published;
		readonly IList<IReceived> _received;
		Stopwatch _timer;

		ReceiveContext()
		{
			Id = CombGuid.Generate();
			_timer = Stopwatch.StartNew();
			_sent = new List<ISent>();
			_published = new List<IPublished>();
			_received = new List<IReceived>();
		}

		ReceiveContext(Stream bodyStream)
			: this()
		{
			_bodyStream = bodyStream;
		}

		/// <summary>
		/// The endpoint from which the message was received
		/// </summary>
		public IEndpoint Endpoint { get; private set; }

		public IReceiveContext BaseContext
		{
			get { return this; }
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

		public void SetBodyStream(Stream stream)
		{
			_bodyStream = stream;
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

		public void SetMessageTypeConverter(IMessageTypeConverter serializer)
		{
			_typeConverter = serializer;
		}

		public void NotifySend(ISendContext context, IEndpointAddress address)
		{
			_sent.Add(new Sent(context, address, _timer.ElapsedMilliseconds));
		}

		public void NotifySend<T>(ISendContext<T> sendContext, IEndpointAddress address)
			where T : class
		{
			_sent.Add(new Sent<T>(sendContext, address, _timer.ElapsedMilliseconds));
		}

		public void NotifyPublish<T>(IPublishContext<T> publishContext)
			where T : class
		{
			_published.Add(new Published<T>(publishContext));
		}

		public void NotifyConsume<T>(IConsumeContext<T> consumeContext, string consumerType, string correlationId) 
			where T : class
		{
			if (SpecialLoggers.Messages.IsInfoEnabled)
				SpecialLoggers.Messages.InfoFormat("RECV:{0}:{1}:{2}", consumeContext.InputAddress, typeof(T).ToMessageName(),
					consumeContext.MessageId);

			_received.Add(new Received<T>(consumeContext, consumerType, correlationId, _timer.ElapsedMilliseconds));
		}

		public IEnumerable<ISent> Sent
		{
			get { return _sent; }
		}

		public IEnumerable<IReceived> Received
		{
			get { return _received; }
		}

		public Guid Id { get; private set; }

		public bool TryGetContext<T>(out IConsumeContext<T> context)
			where T : class
		{
			try
			{
				T message;
				if (_typeConverter.TryConvert(out message))
				{
					context = new ConsumeContext<T>(this, message);
					return true;
				}
			}
			catch (Exception ex)
			{
				var exception = new SerializationException("Failed to deserialize the message", ex);

				_log.Error("Exception converting message to type: " + typeof (T).ToShortTypeName(), exception);
			}

			context = null;
			return false;
		}

		static readonly ILog _log = LogManager.GetLogger(typeof (ReceiveContext));
		
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
				Bus.GetEndpoint(ResponseAddress).Send(message, context =>
					{
						context.SetSourceAddress(Bus.Endpoint.Address.Uri);
						contextCallback(context);
					});
			}
			else
			{
				Bus.Publish(message, context =>
					{
						// don't roll this up or it breaks the 3.5 build
						contextCallback(context);
					});
			}
		}

		[UsedImplicitly]
		void SendFault<T>(T message)
			where T : class
		{
			if (FaultAddress != null)
			{
				Bus.GetEndpoint(FaultAddress).Send(message, context => context.SetSourceAddress(Bus.Endpoint.Address.Uri));
			}
			else if (ResponseAddress != null)
			{
				Bus.GetEndpoint(ResponseAddress).Send(message, context => context.SetSourceAddress(Bus.Endpoint.Address.Uri));
			}
			else
			{
				Bus.Publish(message);
			}
		}

		public static ReceiveContext FromBodyStream(Stream bodyStream)
		{
			return new ReceiveContext(bodyStream);
		}

		public static ReceiveContext Empty()
		{
			return new ReceiveContext(null);
		}
	}

	public class ConsumeContext<TMessage> :
		IConsumeContext<TMessage>
		where TMessage : class
	{
		static readonly ILog _log = LogManager.GetLogger(typeof (ReceiveContext));

		readonly IReceiveContext _context;
		readonly TMessage _message;
		Uri _responseAddress;

		public ConsumeContext(IReceiveContext context, TMessage message)
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

		public string ContentType
		{
			get { return _context.ContentType; }
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
			get { return _context.Network; }
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

		public IEndpoint Endpoint
		{
			get { return _context.Endpoint; }
		}

		public Uri InputAddress
		{
			get { return _context.InputAddress; }
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
				context = new ConsumeContext<T>(_context, Message as T);
				return true;
			}

			context = null;
			return false;
		}

		public IReceiveContext BaseContext
		{
			get { return _context; }
		}

		public void RetryLater()
		{
			if (_log.IsDebugEnabled)
				_log.DebugFormat("Retrying message of type {0} later", typeof (TMessage));

			Bus.Endpoint.Send(Message, x =>
				{
					x.SetUsing(this);
					x.SetRetryCount(RetryCount + 1);
				});
		}

		public void Respond<T>(T message, Action<ISendContext<T>> contextCallback)
			where T : class
		{
			_context.Respond(message, contextCallback);
		}

		public void GenerateFault(Exception ex)
		{
			if (Message == null)
				throw new InvalidOperationException("A fault cannot be generated when no message is present");

			object message =
				FastActivator.Create(Message.Implements(typeof (CorrelatedBy<>)) ? typeof (Fault<,>) : typeof (Fault<>),
					Message, ex);

			this.FastInvoke("SendFault", message);
		}

		public void SetResponseAddress(Uri value)
		{
			_responseAddress = value;
		}

		[UsedImplicitly]
		void SendFault<T>(T message)
			where T : class
		{
			if (FaultAddress != null)
			{
				Bus.GetEndpoint(FaultAddress).Send(message, context => context.SetSourceAddress(Bus.Endpoint.Address.Uri));
			}
			else if (ResponseAddress != null)
			{
				Bus.GetEndpoint(ResponseAddress).Send(message, context => context.SetSourceAddress(Bus.Endpoint.Address.Uri));
			}
			else
			{
				Bus.Publish(message);
			}
		}
	}
}
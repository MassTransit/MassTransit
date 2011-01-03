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
namespace MassTransit.Internal
{
	using System;
	using System.Diagnostics;
	using System.Runtime.Serialization;
	using log4net;
	using Serialization;
	using Transports;

    [DebuggerDisplay("{Address}")]
    public class Endpoint :
		IEndpoint
	{
		private static readonly ILog _log = LogManager.GetLogger(typeof (Endpoint));
		private readonly IEndpointAddress _address;
        protected ITransport _transport;
        protected ITransport _errorTransport;
        protected bool _disposed;
		private string _disposedMessage;
        MessageRetryTracker _tracker;

        public Endpoint(IEndpointAddress address, IMessageSerializer serializer, ITransport transport, ITransport errorTransport)
		{
			_address = address;
		    _transport = transport;
		    _errorTransport = errorTransport;
		    Serializer = serializer;

            _tracker = new MessageRetryTracker(5);

            SetDisposedMessage();
		}

		protected IMessageSerializer Serializer { get; set; }

		public IEndpointAddress Address
		{
			get { return _address; }
		}

		public Uri Uri
		{
			get { return Address.Uri; }
		}

        public void Send<T>(T message) where T : class
		{
			if (_disposed) throw NewDisposedException();

			_transport.Send(context =>
				{
					SetOutboundMessageHeaders<T>();

					PopulateTransportMessage(context, message);

					if (SpecialLoggers.Messages.IsInfoEnabled)
						SpecialLoggers.Messages.InfoFormat("SEND:{0}:{1}", Address, typeof (T).Name);
				});
		}

        public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected void SetDisposedMessage()
		{
			_disposedMessage = "The endpoint has already been disposed: " + _address;
		}

		protected ObjectDisposedException NewDisposedException()
		{
			return new ObjectDisposedException(_disposedMessage);
		}

		protected void SetOutboundMessageHeaders<T>()
		{
			OutboundMessage.Set(headers =>
				{
					headers.SetMessageType(typeof (T));
					headers.SetDestinationAddress(Uri);
				});
		}

		protected virtual void Dispose(bool disposing)
		{
			if (_disposed) return;
			if (disposing)
			{
				_transport.Dispose();
				_transport = null;

				_errorTransport.Dispose();
				_errorTransport = null;

				Serializer = null;
			}

			_disposed = true;
		}

		~Endpoint()
		{
			Dispose(false);
		}

	    protected void PopulateTransportMessage<T>(ISendingContext sendingContext, T message)
	    {
	        Serializer.Serialize(sendingContext.Body, message);


	        sendingContext.SetLabel(typeof(T).Name);
	        sendingContext.MarkRecoverable();

	        if(OutboundMessage.Headers.ExpirationTime.HasValue)
	            sendingContext.SetMessageExpiration(OutboundMessage.Headers.ExpirationTime.Value);
	    }

        protected void MoveMessageToErrorTransport(IReceivingContext message)
        {
            _errorTransport.Send(context=>
            {
                message.Body.CopyTo(context.Body);
                if(OutboundMessage.Headers.ExpirationTime.HasValue)
                    context.SetMessageExpiration(OutboundMessage.Headers.ExpirationTime.Value);
            });

            if (_log.IsDebugEnabled)
                _log.DebugFormat("MOVE:{0}:{1}:{2}", Address, _errorTransport.Address, message.GetMessageId());

            if (SpecialLoggers.Messages.IsInfoEnabled)
                SpecialLoggers.Messages.InfoFormat("MOVE:{0}:{1}:{2}", Address, _errorTransport.Address, message.GetMessageId());
        }

        public virtual void Receive(Func<object, Action<object>> receiver)
        {
            if (_disposed) throw NewDisposedException();

            _transport.Receive(ReceiveFromTransport(receiver), TimeSpan.Zero);
        }

        public virtual void Receive(Func<object, Action<object>> receiver, TimeSpan timeout)
        {
            if (_disposed) throw NewDisposedException();

            _transport.Receive(ReceiveFromTransport(receiver), timeout);
        }

        Func<IReceivingContext, Action<IReceivingContext>> ReceiveFromTransport(Func<object, Action<object>> receiver)
        {
            return receivingContext =>
            {
                if(_tracker.IsRetryLimitExceeded(receivingContext.GetMessageId()))
                {
                    if(_log.IsErrorEnabled)
                        _log.ErrorFormat("Message retry limit exceeded {0}:{1}", Address, receivingContext.GetMessageId());

                    return MoveMessageToErrorTransport;
                }

                object messageObj;

                try
                {
                    messageObj = Serializer.Deserialize(receivingContext.Body);
                }
                catch (SerializationException sex)
                {
                    if (_log.IsErrorEnabled)
                        _log.Error("Unrecognized message " + Address + ":" + receivingContext.GetMessageId(), sex);

                    _tracker.IncrementRetryCount(receivingContext.GetMessageId());
                    return MoveMessageToErrorTransport;
                }

                if (messageObj == null)
                    return null;

                Action<object> receive;
                try
                {
                    receive = receiver(messageObj);
                    if (receive == null)
                    {
                        if (_log.IsDebugEnabled)
                            _log.DebugFormat("SKIP:{0}:{1}", Address, messageObj.GetType().Name);

                        if (SpecialLoggers.Messages.IsInfoEnabled)
                            SpecialLoggers.Messages.InfoFormat("SKIP:{0}:{1}", Address, messageObj.GetType().Name);

                        return null;
                    }
                }
                catch (Exception ex)
                {
                    if (_log.IsErrorEnabled)
                        _log.Error("An exception was thrown preparing the message consumers", ex);

                    _tracker.IncrementRetryCount(receivingContext.GetMessageId());
                    return null;
                }

                return m =>
                {
                    if (_log.IsDebugEnabled)
                        _log.DebugFormat("RECV:{0}:{1}:{2}", Address, m.GetMessageId(), messageObj.GetType().Name);

                    if (SpecialLoggers.Messages.IsInfoEnabled)
                        SpecialLoggers.Messages.InfoFormat("RECV:{0}:{1}", Address, messageObj.GetType().Name);

                    try
                    {
                        receive(messageObj);

                        _tracker.MessageWasReceivedSuccessfully(receivingContext.GetMessageId());
                    }
                    catch (Exception ex)
                    {
                        if (_log.IsErrorEnabled)
                            _log.Error("An exception was thrown by a message consumer", ex);

                        _tracker.IncrementRetryCount(receivingContext.GetMessageId());
                        MoveMessageToErrorTransport(m);
                    }
                };
            };
        }
	}
}
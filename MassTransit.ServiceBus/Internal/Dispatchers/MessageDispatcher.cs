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
namespace MassTransit.ServiceBus.Internal
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using Exceptions;
    using log4net;

    public class MessageDispatcher<TMessage> :
        IMessageDispatcher<TMessage>
        where TMessage : class
    {
        private static readonly ILog _log = LogManager.GetLogger(typeof (MessageDispatcher<TMessage>));
        private readonly List<Consumes<TMessage>.All> _consumers = new List<Consumes<TMessage>.All>();
    	private readonly ReaderWriterLock _consumerLock = new ReaderWriterLock();

        public bool Accept(TMessage message)
        {
            if (_log.IsDebugEnabled && _consumers.Count == 0)
                _log.DebugFormat("No consumers for message type {0}", typeof (TMessage));

            IList<Consumes<TMessage>.All> consumers;
        	_consumerLock.AcquireReaderLock(Timeout.Infinite);
        	try
        	{
				consumers = new List<Consumes<TMessage>.All>(_consumers);
        	}
        	finally
        	{
        		_consumerLock.ReleaseReaderLock();
        	}

            foreach (Consumes<TMessage>.All consumer in consumers)
            {
                Consumes<TMessage>.Selected selectiveConsumer = consumer as Consumes<TMessage>.Selected;
                if (selectiveConsumer == null)
                {
                    // they aren't selective, so return true
                    return true;
                }

                // if the consumer is selective, ask if they want it and if so return true
                if (selectiveConsumer.Accept(message))
                    return true;
            }

            return false;
        }

        public void Consume(TMessage message)
        {
            IList<Consumes<TMessage>.All> consumers;
			_consumerLock.AcquireReaderLock(Timeout.Infinite);
			try
			{
				consumers = new List<Consumes<TMessage>.All>(_consumers);
			}
			finally
			{
				_consumerLock.ReleaseReaderLock();
			}

            foreach (Consumes<TMessage>.All consumer in consumers)
            {
                Consumes<TMessage>.Selected selectiveConsumer = consumer as Consumes<TMessage>.Selected;
                if (selectiveConsumer == null)
                {
                    consumer.Consume(message);
                }
                else
                {
                    if (selectiveConsumer.Accept(message))
                    {
                        consumer.Consume(message);
                    }
                }
            }
        }

        public bool Accept(object obj)
        {
            TMessage message = GetTypedMessage(obj);

            return Accept(message);
        }

        public void Consume(object obj)
        {
            TMessage message = GetTypedMessage(obj);

            Consume(message);
        }

        public bool Active
        {
            get { return _consumers.Count > 0; }
        }

        public T GetDispatcher<T>() where T : class
        {
			_consumerLock.AcquireReaderLock(Timeout.Infinite);
			try
			{
                foreach (Consumes<TMessage>.All consumer in _consumers)
                {
                    if (consumer.GetType() == typeof(T))
                        return (T)consumer;
                }
			}
			finally
			{
				_consumerLock.ReleaseReaderLock();
			}

            return default(T);
        }

        public T GetDispatcher<T>(Type type) where T : class
        {
			_consumerLock.AcquireReaderLock(Timeout.Infinite);
			try
			{
                foreach (Consumes<TMessage>.All consumer in _consumers)
                {
                    if (consumer.GetType() == type)
                        return (T)consumer;
                }
			}
			finally
			{
				_consumerLock.ReleaseReaderLock();
			}

            return default(T);
        }

        public void Dispose()
        {
            _consumers.Clear();
        }

        public void Attach(Consumes<TMessage>.All consumer)
        {
			_consumerLock.AcquireReaderLock(Timeout.Infinite);
			try
			{
				if (_consumers.Contains(consumer))
					return;

				LockCookie cookie = _consumerLock.UpgradeToWriterLock(Timeout.Infinite);
				try
				{
					if (_consumers.Contains(consumer))
						return;

						_consumers.Add(consumer);
				}
				finally
				{
					_consumerLock.DowngradeFromWriterLock(ref cookie);
				}
			}
			finally
			{
				_consumerLock.ReleaseReaderLock();
			}
        }

        public void Detach(Consumes<TMessage>.All consumer)
        {
			_consumerLock.AcquireReaderLock(Timeout.Infinite);
			try
			{
				if (!_consumers.Contains(consumer))
					return;

				LockCookie cookie = _consumerLock.UpgradeToWriterLock(Timeout.Infinite);
				try
				{
					if (!_consumers.Contains(consumer))
						return;

					_consumers.Remove(consumer);
				}
				finally
				{
					_consumerLock.DowngradeFromWriterLock(ref cookie);
				}
			}
			finally
			{
				_consumerLock.ReleaseReaderLock();
			}
        }

        private static TMessage GetTypedMessage(object obj)
        {
            TMessage message = obj as TMessage;
            if (message == null)
                throw new ConventionException("The message is not of type " + typeof (TMessage).FullName);

            return message;
        }
    }
}
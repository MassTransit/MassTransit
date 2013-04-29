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
namespace MassTransit.Transports
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using Context;
    using Loopback;
    using Magnum.Extensions;
    using Subscriptions.Coordinator;

    /// <summary>
    /// The loopback transport is a built-in transport for MassTransit that 
    /// works on messages in-memory. It is dependent on the <see cref="SubscriptionLoopback"/>
    /// that takes care of subscribing the buses in the process
    /// depending on what subscriptions are made.
    /// </summary>
    public class LoopbackTransport :
        IDuplexTransport
    {
        readonly object _messageReadLock = new object();
        readonly object _messageWriteLock = new object();
        readonly TimeSpan _deadlockTimeout = new TimeSpan(0, 1, 0);
        bool _disposed;
        AutoResetEvent _messageReady = new AutoResetEvent(false);
        LinkedList<LoopbackMessage> _messages = new LinkedList<LoopbackMessage>();

        public LoopbackTransport(IEndpointAddress address)
        {
            Address = address;
        }

        public int Count
        {
            get
            {
                int messageCount;
                if (!Monitor.TryEnter(_messageReadLock, _deadlockTimeout))
                    throw new Exception("Deadlock detected!");

                try
                {
                    GuardAgainstDisposed();

                    messageCount = _messages.Count;
                }
                finally
                {
                    Monitor.Exit(_messageReadLock);
                }

                return messageCount;
            }
        }

        public IEndpointAddress Address { get; private set; }

        public IOutboundTransport OutboundTransport
        {
            get { return this; }
        }

        public IInboundTransport InboundTransport
        {
            get { return this; }
        }

        public void Send(ISendContext context)
        {
            GuardAgainstDisposed();

            LoopbackMessage message = null;
            try
            {
                message = new LoopbackMessage();

                if (context.ExpirationTime.HasValue)
                {
                    message.ExpirationTime = context.ExpirationTime.Value;
                }

                context.SerializeTo(message.Body);
                message.ContentType = context.ContentType;
                message.OriginalMessageId = context.OriginalMessageId;

                if (!Monitor.TryEnter(_messageWriteLock, _deadlockTimeout))
                    throw new Exception("Deadlock detected!");

                try
                {
                    GuardAgainstDisposed();

                    _messages.AddLast(message);
                }
                finally
                {
                    Monitor.Exit(_messageWriteLock);
                }

                Address.LogSent(message.MessageId, context.MessageType);
            }
            catch
            {
                if (message != null)
                    message.Dispose();

                throw;
            }

            _messageReady.Set();
        }

        public void Receive(Func<IReceiveContext, Action<IReceiveContext>> callback, TimeSpan timeout)
        {
            int messageCount = Count;

            bool waited = false;

            if (messageCount == 0)
            {
                if (!_messageReady.WaitOne(timeout, true))
                    return;

                waited = true;
            }

            bool monitorExitNeeded = true;
            if (!Monitor.TryEnter(_messageReadLock, timeout))
                return;

            try
            {
                for (LinkedListNode<LoopbackMessage> iterator = _messages.First;
                     iterator != null;
                     iterator = iterator.Next)
                {
                    if (iterator.Value != null)
                    {
                        LoopbackMessage message = iterator.Value;
                        if (message.ExpirationTime.HasValue && message.ExpirationTime <= DateTime.UtcNow)
                        {
                            if (!Monitor.TryEnter(_messageWriteLock, _deadlockTimeout))
                                throw new Exception("Deadlock detected!");

                            try
                            {
                                _messages.Remove(iterator);
                            }
                            finally
                            {
                                Monitor.Exit(_messageWriteLock);
                            }
                            return;
                        }

                        ReceiveContext context = ReceiveContext.FromBodyStream(message.Body);
                        context.SetMessageId(message.MessageId);
                        context.SetContentType(message.ContentType);
                        context.SetOriginalMessageId(message.OriginalMessageId);
                        if (message.ExpirationTime.HasValue)
                            context.SetExpirationTime(message.ExpirationTime.Value);

                        Action<IReceiveContext> receive = callback(context);
                        if (receive == null)
                            continue;

                        if (!Monitor.TryEnter(_messageWriteLock, _deadlockTimeout))
                            throw new Exception("Deadlock detected!");

                        try
                        {
                            _messages.Remove(iterator);
                        }
                        finally
                        {
                            Monitor.Exit(_messageWriteLock);
                        }

                        using (message)
                        {
                            Monitor.Exit(_messageReadLock);
                            monitorExitNeeded = false;

                            receive(context);
                            return;
                        }
                    }
                }

                if (waited)
                    return;

                // we read to the end and none were accepted, so we are going to wait until we get another in the queue
                // make any other potential readers wait as well
                _messageReady.WaitOne(timeout, true);
            }
            finally
            {
                if (monitorExitNeeded)
                    Monitor.Exit(_messageReadLock);
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }

        void GuardAgainstDisposed()
        {
            if (_disposed)
                throw new ObjectDisposedException("The transport has already been disposed: " + Address);
        }

        void Dispose(bool disposing)
        {
            if (_disposed)
                return;
            if (disposing)
            {
                lock (_messageReadLock)
                {
                    lock (_messageWriteLock)
                    {
                        _messages.Each(x => x.Dispose());
                        _messages.Clear();
                        _messages = null;
                    }
                }

                _messageReady.Close();
                using (_messageReady)
                {
                }
                _messageReady = null;
            }

            _disposed = true;
        }
    }
}
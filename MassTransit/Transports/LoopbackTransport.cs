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
namespace MassTransit.Transports
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Threading;

    [DebuggerDisplay("{Address}")]
    public class LoopbackTransport :
        ITransport
    {
        private readonly object _messageLock = new object();
        private bool _disposed;
        private ManualResetEvent _messageReady = new ManualResetEvent(false);
        private LinkedList<MemoryStream> _messages = new LinkedList<MemoryStream>();

        public LoopbackTransport(IEndpointAddress address)
        {
            Address = address;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public IEndpointAddress Address { get; private set; }

        public void Send(Action<Stream> sender)
        {
            if (_disposed) throw NewDisposedException();

            MemoryStream bodyStream = null;
            try
            {
                bodyStream = new MemoryStream();

                sender(bodyStream);

                lock (_messageLock)
                {
                    if (_disposed) throw NewDisposedException();

                    _messages.AddLast(bodyStream);
                }

                bodyStream = null;
            }
            finally
            {
                if (bodyStream != null)
                    bodyStream.Dispose();
            }

            _messageReady.Set();
        }

        public void Receive(Func<Stream, Action<Stream>> receiver)
        {
            if (_disposed) throw NewDisposedException();

            Receive(receiver, TimeSpan.Zero);
        }

        public void Receive(Func<Stream, Action<Stream>> receiver, TimeSpan timeout)
        {
            if (_disposed) throw NewDisposedException();

            int messageCount;
            lock (_messageLock)
            {
                if (_disposed)
                    return;

                messageCount = _messages.Count;
            }

            if (messageCount == 0)
            {
                if (!_messageReady.WaitOne(timeout, true))
                    return;
            }

            bool monitorExitNeeded = true;
            if (!Monitor.TryEnter(_messageLock, timeout))
                return;

            try
            {
                for (LinkedListNode<MemoryStream> iterator = _messages.First; iterator != null; iterator = iterator.Next)
                {
                    iterator.Value.Seek(0, SeekOrigin.Begin);

                    Action<Stream> receive = receiver(iterator.Value);
                    if (receive == null)
                        continue;

                    MemoryStream message = iterator.Value;
                    message.Seek(0, SeekOrigin.Begin);

                    _messages.Remove(iterator);

                    try
                    {
                        Monitor.Exit(_messageLock);
                        monitorExitNeeded = false;

                        receive(message);
                        return;
                    }
                    finally
                    {
                        message.Dispose();
                    }
                }
            }
            finally
            {
                if (monitorExitNeeded)
                    Monitor.Exit(_messageLock);
            }

            lock (_messageLock)
                messageCount = _messages.Count;

            if (messageCount == 0)
                _messageReady.WaitOne(timeout, true);
        }

        private void Dispose(bool disposing)
        {
            if (_disposed) return;
            if (disposing)
            {
                lock (_messageLock)
                {
                    _messages.Each(x => x.Dispose());
                    _messages.Clear();
                    _messages = null;
                }

                _messageReady.Close();
                _messageReady = null;
            }

            _disposed = true;
        }

        private ObjectDisposedException NewDisposedException()
        {
            return new ObjectDisposedException("The transport has already been disposed: " + Address);
        }

        ~LoopbackTransport()
        {
            Dispose(false);
        }
    }
}
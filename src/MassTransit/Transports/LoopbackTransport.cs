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
    using System.IO;
    using System.Threading;

    public class LoopbackTransport :
        TransportBase
    {
        private readonly object _messageLock = new object();
        private AutoResetEvent _messageReady = new AutoResetEvent(false);
        private LinkedList<MemoryStream> _messages = new LinkedList<MemoryStream>();

        public LoopbackTransport(IEndpointAddress address) : base(address)
        {
        }

        public override void Send(Action<Stream> sender)
        {
            EnsureNotDisposed();

            MemoryStream bodyStream = null;
            try
            {
                bodyStream = new MemoryStream();

                sender(bodyStream);

                lock (_messageLock)
                {
                    EnsureNotDisposed();

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

        public override void Receive(Func<Stream, Action<Stream>> receiver, TimeSpan timeout)
        {
            EnsureNotDisposed();

            int messageCount;
            lock (_messageLock)
            {
                if (_disposed)
                    return;

                messageCount = _messages.Count;
            }

        	bool waited = false;

            if (messageCount == 0)
            {
                if (!_messageReady.WaitOne(timeout, true))
                    return;

            	waited = true;
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

				if(waited)
					return;

				// we read to the end and none were accepted, so we are going to wait until we get another in the queue
				if (!_messageReady.WaitOne(timeout, true))
					return;
            }
            finally
            {
                if (monitorExitNeeded)
                    Monitor.Exit(_messageLock);
            }
        }

        public override void OnDisposing()
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
    }
}
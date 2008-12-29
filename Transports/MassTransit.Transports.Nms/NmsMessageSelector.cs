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
namespace MassTransit.Transports.Nms
{
    using System;
    using System.IO;
    using System.Runtime.Serialization;
    using Apache.NMS;
    using Exceptions;
    using Internal;
    using Serialization;

    public class NmsMessageSelector :
        IMessageSelector
    {
        private readonly NmsEndpoint _endpoint;
        private readonly IMessageSerializer _serializer;
        private readonly ISession _session;
        private readonly IMessage _transportMessage;
        private volatile bool _disposed;
        private object _message;

        public NmsMessageSelector(NmsEndpoint endpoint, ISession session, IMessage transportMessage, IMessageSerializer serializer)
        {
            _endpoint = endpoint;
            _session = session;
            _transportMessage = transportMessage;
            _serializer = serializer;
        }

        public bool AcceptMessage()
        {
            if (_disposed) throw new ObjectDisposedException("The object has been disposed");

            _session.Commit();

            return true;
        }

        public void MoveMessageTo(IEndpoint endpoint)
        {
            if (_disposed) throw new ObjectDisposedException("The object has been disposed");

            if (_message == null)
                throw new EndpointException(_endpoint, "Unable to move message since it has not been deserialized");

            endpoint.Send(_message, _transportMessage.NMSTimeToLive);
        }

        public object DeserializeMessage()
        {
            if (_disposed) throw new ObjectDisposedException("The object has been disposed");

            try
            {
                IBytesMessage bm = _transportMessage as IBytesMessage;
                if (bm == null)
                    throw new MessageException(_transportMessage.GetType(), "Message not a IBytesMessage");

                using (MemoryStream mem = new MemoryStream(bm.Content, false))
                {
                    _message = _serializer.Deserialize(mem);
                }

                return _message;
            }
            catch (SerializationException ex)
            {
                // if we get a message we cannot serialize, we need to do something about it or it will 
                // hang the service bus forever

                // _endpoint.DiscardMessage(_transportMessage.Id, ex.Message);

                throw new MessageException(typeof (object), "An error occurred deserializing a message", ex);
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed) return;
            if (disposing)
            {
            }

            _disposed = true;
        }

        ~NmsMessageSelector()
        {
            Dispose(false);
        }
    }
}